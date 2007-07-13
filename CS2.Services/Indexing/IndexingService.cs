using System;
using System.IO;
using System.Text.RegularExpressions;
using CS2.Services.Parsing;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Directory=Lucene.Net.Store.Directory;

namespace CS2.Services.Indexing
{
    public class IndexingService : IIndexingService
    {
        #region Fields

        private readonly IParsingService[] parsingServices;
        private readonly Directory indexDirectory;
        private readonly SynchronizedSet filesWaitingToBeIndexed;
        private IndexReader indexReader;
        private IndexWriter indexWriter;
        private bool isUpdating = false;
        private string[] exclusions;
        private int addedFilesSinceLastUpdate;
        private int deletedFilesSinceLastUpdate;
        private readonly object updatingLock = new object();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexingService"/> class.
        /// </summary>
        /// <param name="indexDirectory">The index directory.</param>
        /// <param name="parsingServices">The parsing services.</param>
        public IndexingService(Directory indexDirectory, IParsingService[] parsingServices)
        {
            filesWaitingToBeIndexed = new SynchronizedSet();
            this.indexDirectory = indexDirectory;
            this.parsingServices = parsingServices;

            // If the index directory doesn't contain an index then create it
            if (!IndexReader.IndexExists(indexDirectory))
            {
                IndexWriter writer = new IndexWriter(indexDirectory, new StandardAnalyzer(), true);
                writer.Optimize();
                writer.Close();
            }
        }

        #region IIndexingService Members

        /// <summary>
        /// Returns the number of files deleted from the index since last update.
        /// </summary>
        /// <value></value>
        public int DeletedFilesSinceLastUpdate
        {
            get { return deletedFilesSinceLastUpdate; }
        }

        /// <summary>
        /// Returns the number of files added to the index since last update.
        /// </summary>
        /// <value></value>
        public int AddedFilesSinceLastUpdate
        {
            get { return addedFilesSinceLastUpdate; }
        }

        /// <summary>
        /// Returns true if there are files waiting to be indexed in batch.
        /// </summary>
        /// <value></value>
        public bool IsWaitingForFilesToBeIndexed
        {
            get { return filesWaitingToBeIndexed.Count != 0; }
        }

        /// <summary>
        /// The directory where the index is located.
        /// </summary>
        /// <value></value>
        public Directory IndexDirectory
        {
            get { return indexDirectory; }
        }

        public string[] Exclusions
        {
            get { return exclusions; }
            set { exclusions = value; }
        }

        /// <summary>
        /// Requests the indexing of the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void RequestIndexing(FileInfo file)
        {
            if(!IsValidFileSystemEntryToBeIndexed(file))
                return;

            // Add the file to be indexed to the queue, if it doesn't contain the file yet
            filesWaitingToBeIndexed.Add(file.FullName);
        }

        /// <summary>
        /// Requests the indexing of all the files contained in the specified directory and all its subdirectories.
        /// </summary>
        /// <param name="directory">The directory.</param>
        public void RequestIndexing(DirectoryInfo directory)
        {
            if(!IsValidFileSystemEntryToBeIndexed(directory))
                return;

            foreach(FileInfo file in directory.GetFiles("*", SearchOption.AllDirectories))
                RequestIndexing(file);
        }

        /// <summary>
        /// Triggers update operations on the index and on the files repository,
        /// removing no longer existing files references both from repository and index,
        /// updating changed documents and adding new documents which have been explicitly required to be indexed.
        /// </summary>
        public void UpdateIndex()
        {
            lock(updatingLock)
            {
                if(isUpdating)
                    return;

                isUpdating = true;
            }

            SynchronizedSet filesUndergoingIndexing = filesWaitingToBeIndexed.CloneAndClear();

            int addedFiles = 0;
            int deletedFiles = 0;

            // Create new IndexReader to update the index
            indexReader = IndexReader.Open(indexDirectory);

            RemoveOldEntries(filesUndergoingIndexing, ref deletedFiles);

            // Close the IndexReader
            indexReader.Close();
            indexReader = null;

            if(filesUndergoingIndexing.Count > 0)
            {
                // Create a new IndexWriter to add new documents to the index
                indexWriter = new IndexWriter(indexDirectory, new StandardAnalyzer(), false);

                foreach(string fileName in filesUndergoingIndexing)
                    if(Index(new FileInfo(fileName)))
                        addedFiles++;

                // Close the IndexWriter
                indexWriter.Optimize();
                indexWriter.Close();
                indexWriter = null;
            }

            addedFilesSinceLastUpdate = addedFiles;
            deletedFilesSinceLastUpdate = deletedFiles;

            // Fire IndexingCompleted event
            OnIndexingCompleted();

            lock(updatingLock)
                isUpdating = false;
        }

        /// <summary>
        /// Occurs when indexing is completed.
        /// </summary>
        public event EventHandler<IndexingCompletedEventArgs> IndexingCompleted;

        #endregion

        /// <summary>
        /// Determines whether the specified entry is a valid file system entry to be indexed.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid file system entry to be indexed] [the specified entry]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidFileSystemEntryToBeIndexed(FileSystemInfo entry)
        {
            // The file or directory doesn't exist
            if(!entry.Exists)
                return false;

            // The file or directory is hidden
            if(entry.Attributes == FileAttributes.Hidden)
                return false;

            // The entry matches one of the exclusions
            if(MatchesAnyExclusion(entry, exclusions))
                return false;

            return true;
        }

        /// <summary>
        /// Returns true if the supplied <see cref="System.IO.FileSystemInfo"/> 
        /// matches any of the patterns in the <paramref name="exclusions"/>, false otherwise.
        /// </summary>
        /// <param name="entry">The entry in the file system.</param>
        /// <param name="exclusions">The array of exclusions.</param>
        /// <returns></returns>
        private static bool MatchesAnyExclusion(FileSystemInfo entry, string[] exclusions)
        {
            return !Array.TrueForAll(exclusions, delegate(string exclusion)
                {
                    Regex r = new Regex(Regex.Escape(exclusion));
                    return !r.IsMatch(entry.FullName);
                });
        }

        /// <summary>
        /// Called to fire the <see cref="IndexingCompleted" /> event.
        /// </summary>
        private void OnIndexingCompleted()
        {
            if(IndexingCompleted != null)
                IndexingCompleted(this, new IndexingCompletedEventArgs(addedFilesSinceLastUpdate, deletedFilesSinceLastUpdate));
        }

        /// <summary>
        /// Removes the deleted and modified documents from the index. Marks the modified files as to be reindexed.
        /// </summary>
        private void RemoveOldEntries(SynchronizedSet filesUndergoingIndexing, ref int deletedFiles)
        {
            // Create a term enumerator to iterate through all the terms of the ID field
            // This is done to avoid searching, which is presumably less performant
            TermEnum idEnumerator = indexReader.Terms(new Term(FieldFactory.IdFieldName, ""));

            // Iterate all the documents into the index
            while(idEnumerator.Term() != null && idEnumerator.Term().Field() == FieldFactory.IdFieldName)
            {
                string filePath = IdIdentifierUtilities.GetPathFromIdentifier(idEnumerator.Term().Text());

                // If the file is already in the index remove it from the list of the files waiting to be indexed
                filesUndergoingIndexing.Remove(filePath);

                // If file doesn't exist or if file exists but is out of date
                if(!File.Exists(filePath) ||
                   (File.Exists(filePath) &&
                    IdIdentifierUtilities.GetIdentifierFromFile(new FileInfo(filePath)) != idEnumerator.Term().Text()))
                {
                    // The delete document from the index
                    indexReader.DeleteDocuments(idEnumerator.Term());
                    deletedFiles++;

                    // If file was deleted since out of date then re-index it
                    if(File.Exists(filePath))
                        filesUndergoingIndexing.Add(filePath);
                }

                idEnumerator.Next();
            }

            idEnumerator.Close();
        }

        /// <summary>
        /// Indexes the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        private bool Index(FileInfo file)
        {
            Document document;

            // Find a parser that suits the file
            foreach(IParsingService parsingService in parsingServices)
                if(!MatchesAnyExclusion(file, parsingService.Exclusions) && parsingService.TryParse(file, out document))
                {
                    document.Add(FieldFactory.CreateIdField(IdIdentifierUtilities.GetIdentifierFromFile(file)));
                    document.Add(FieldFactory.CreatePathField(file.FullName));
                    document.Add(FieldFactory.CreateFileNameField(file.Name));
                    document.Add(FieldFactory.CreateSourceField(new StreamReader(file.FullName, true)));
                    document.Add(FieldFactory.CreateLanguageField(parsingService.Analyzer.LanguageName));

                    // Add the document to the index with the appropriate analyzer
                    indexWriter.AddDocument(document, parsingService.Analyzer);

                    // If a parser has been able to parse the file stop iterating through parsers and return
                    return true;
                }

            // No parser able to parse the file found, file hasn't been indexed
            return false;
        }
    }
}
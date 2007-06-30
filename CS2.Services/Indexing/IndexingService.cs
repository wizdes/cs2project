using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CS2.Model;
using CS2.Repositories;
using CS2.Services.Parsing;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Directory=Lucene.Net.Store.Directory;

namespace CS2.Services.Indexing
{
    public class IndexingService : IIndexingService
    {
        private readonly IDictionary<string, FileInfo> filesWaitingToBeIndexed;

        private readonly Directory indexDirectory;
        private readonly IParsingService[] parsingServices;
        private readonly IFilesRepository repository;
        private readonly object updatingLock = new object();
        private int addedFilesSinceLastUpdate;
        private int deletedFilesSinceLastUpdate;
        private string[] exclusions;
        private IndexReader indexReader;
        private IndexWriter indexWriter;
        private bool isUpdating = false;
        private int updatedFilesSinceLastUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexingService"/> class.
        /// </summary>
        /// <param name="indexDirectory">The index directory.</param>
        /// <param name="parsingServices">The parsing services.</param>
        /// <param name="repository">The repository.</param>
        public IndexingService(Directory indexDirectory, IParsingService[] parsingServices, IFilesRepository repository)
        {
            filesWaitingToBeIndexed = new SortedDictionary<string, FileInfo>();
            this.indexDirectory = indexDirectory;
            this.parsingServices = parsingServices;
            this.repository = repository;

            // If the index directory doesn't contain an index then create it
            if(!IndexReader.IndexExists(indexDirectory))
            {
                indexWriter = new IndexWriter(indexDirectory, new StandardAnalyzer(), true);
                indexWriter.Close();
                indexWriter = null;
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
        /// Returns the number of files updated in the index since last update.
        /// </summary>
        /// <value></value>
        public int UpdatedFilesSinceLastUpdate
        {
            get { return updatedFilesSinceLastUpdate; }
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
            lock(filesWaitingToBeIndexed)
            {
                if(!filesWaitingToBeIndexed.ContainsKey(file.FullName))
                    filesWaitingToBeIndexed.Add(file.FullName, file);
            }
        }

        /// <summary>
        /// Requests the indexing of the specified directory, optionally using recursion and looking for files which match the supplied pattern.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="searchOption">The search option.</param>
        /// <param name="searchPattern">The search pattern.</param>
        public void RequestIndexing(DirectoryInfo directory, SearchOption searchOption, string searchPattern)
        {
            if(!IsValidFileSystemEntryToBeIndexed(directory))
                return;

            foreach(FileInfo file in directory.GetFiles(searchPattern, searchOption))
                RequestIndexing(file);
        }

        /// <summary>
        /// Requests the indexing of the specified directory, optionally using recursion and looking for files of the specified language.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="searchOption">The search option.</param>
        /// <param name="language">The language.</param>
        public void RequestIndexing(DirectoryInfo directory, SearchOption searchOption, IProgrammingLanguage language)
        {
            if(language != null)
                RequestIndexing(directory, searchOption, language.SearchPattern);
        }

        /// <summary>
        /// Requests the indexing of all the files contained in the specified directory, optionally using recursion.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="searchOption">The search option.</param>
        public void RequestIndexing(DirectoryInfo directory, SearchOption searchOption)
        {
            RequestIndexing(directory, searchOption, "*");
        }

        /// <summary>
        /// Requests the indexing of all the files contained in the specified directory and all its subdirectories.
        /// </summary>
        /// <param name="directory">The directory.</param>
        public void RequestIndexing(DirectoryInfo directory)
        {
            RequestIndexing(directory, SearchOption.AllDirectories);
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

            SortedDictionary<string, FileInfo> indexingQueue;

            lock(filesWaitingToBeIndexed)
            {
                indexingQueue = new SortedDictionary<string, FileInfo>(filesWaitingToBeIndexed);
                filesWaitingToBeIndexed.Clear();
            }

            int addedFiles = 0;
            int updatedFiles = 0;
            int deletedFiles = 0;

            // Create new IndexReader to update the index
            indexReader = IndexReader.Open(indexDirectory);

            RemoveDeletedFilesFromRepository(ref deletedFiles);

            RemoveDeletedAndModifiedDocumentsFromIndex(indexingQueue);

            // Close the IndexReader
            indexReader.Close();
            indexReader = null;

            // Create a new IndexWriter to add new documents to the index
            indexWriter = new IndexWriter(indexDirectory, new StandardAnalyzer(), false);

            AddNewAndUpdatedFilesToTheIndex(indexingQueue, ref addedFiles, ref updatedFiles);

            // Close the IndexWriter
            indexWriter.Optimize();
            indexWriter.Close();
            indexWriter = null;

            indexingQueue.Clear();

            addedFilesSinceLastUpdate = addedFiles;
            updatedFilesSinceLastUpdate = updatedFiles;
            deletedFilesSinceLastUpdate = deletedFiles;

            // Fire IndexingCompletedEvent
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
            if(MatchesExclusions(entry, exclusions))
                return false;

            //                if(r.IsMatch(entry.FullName)) })
            //            foreach(string exclusion in Exclusions)
            //            {
            //                Regex r = new Regex(Regex.Escape(exclusion));
            //
            //                if(r.IsMatch(entry.FullName))
            //                    return false;
            //            }

            FileInfo fileEntry = entry as FileInfo;

            // The entry is a file which is already indexed (if it's out of date it will be automatically updated 
            // during the update process, no need to request it to be indexed again)
            if(fileEntry != null && repository.Contains(fileEntry))
                return false;

            return true;
        }

        /// <summary>
        /// Returns true if the supplied <see cref="System.IO.FileSystemInfo"/> 
        /// matches any of the petterns in the <paramref name="exclusions"/>, false otherwise.
        /// </summary>
        /// <param name="entry">The entry in the file system.</param>
        /// <param name="exclusions">The array of exclusions.</param>
        /// <returns></returns>
        private static bool MatchesExclusions(FileSystemInfo entry, string[] exclusions)
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
                IndexingCompleted(this,
                                  new IndexingCompletedEventArgs(addedFilesSinceLastUpdate, updatedFilesSinceLastUpdate,
                                                                 deletedFilesSinceLastUpdate));
        }

        /// <summary>
        /// Adds new and updated files to the index.
        /// </summary>
        /// <param name="indexingQueue">The indexing queue.</param>
        /// <param name="addedFiles">The added files.</param>
        /// <param name="updatedFiles">The updated files.</param>
        private void AddNewAndUpdatedFilesToTheIndex(IDictionary<string, FileInfo> indexingQueue, ref int addedFiles,
                                                     ref int updatedFiles)
        {
            FileInfo[] filesToBeIndexed = new FileInfo[indexingQueue.Count];
            indexingQueue.Values.CopyTo(filesToBeIndexed, 0);

            foreach(FileInfo info in filesToBeIndexed)
                Index(info, ref addedFiles, ref updatedFiles);
        }

        /// <summary>
        /// Removes the deleted and modified documents from the index. Marks the modified files as to be reindexed.
        /// </summary>
        private void RemoveDeletedAndModifiedDocumentsFromIndex(IDictionary<string, FileInfo> indexingQueue)
        {
            // Create a term enumerator to iterate through all the terms of the ID field
            // This is done to avoid searching, which is presumably less performant
            TermEnum idEnumerator = indexReader.Terms(new Term(FieldFactory.IdFieldName, ""));

            // Remove files no longer existing or out of date from the index
            while(idEnumerator.Term() != null && idEnumerator.Term().Field() == FieldFactory.IdFieldName)
            {
                string id = idEnumerator.Term().Text();

                // If file doesn't exist or if file exists but is out of date
                if(!File.Exists(IdIdentifierUtilities.GetPathFromIdentifier(id)) || FileExistsButNeedsUpdating(id))
                {
                    // The delete document from the index
                    indexReader.DeleteDocuments(idEnumerator.Term());

                    // If file was deleted since out of date then re-index it
                    if(FileExistsButNeedsUpdating(id))
                    {
                        FileInfo file = new FileInfo(IdIdentifierUtilities.GetPathFromIdentifier(id));

                        if(!indexingQueue.ContainsKey(file.FullName))
                            indexingQueue.Add(file.FullName, file);
                    }
                }

                idEnumerator.Next();
            }

            idEnumerator.Close();
        }

        /// <summary>
        /// Removes the deleted files from repository.
        /// </summary>
        private void RemoveDeletedFilesFromRepository(ref int deletedFiles)
        {
            // Iterate through the list of files to be indexed and remove them from the repository if they no longer exist
            foreach(string file in repository.GetAll())
                if(!File.Exists(file))
                {
                    repository.Remove(new FileInfo(file));
                    deletedFiles++;
                }
        }

        /// <summary>
        /// Checks whether the file exists but needs to be re-indexed because of an update.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private static bool FileExistsButNeedsUpdating(string id)
        {
            string path = IdIdentifierUtilities.GetPathFromIdentifier(id);

            return File.Exists(path) && IdIdentifierUtilities.GetIdentifierFromFile(new FileInfo(path)) != id;
        }

        /// <summary>
        /// Indexes the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="addedFiles">The added files.</param>
        /// <param name="updatedFiles">The updated files.</param>
        private void Index(FileInfo file, ref int addedFiles, ref int updatedFiles)
        {
            Document document;

            // Find a parser that suites the file
            foreach(IParsingService parsingService in parsingServices)
                if(!MatchesExclusions(file, parsingService.Exclusions) && parsingService.TryParse(file, out document))
                {
                    document.Add(FieldFactory.CreateIdField(IdIdentifierUtilities.GetIdentifierFromFile(file)));
                    document.Add(FieldFactory.CreatePathField(file.FullName));
                    document.Add(FieldFactory.CreateFileNameField(file.Name));
                    document.Add(FieldFactory.CreateSourceField(new StreamReader(file.FullName, true)));
                    document.Add(
                        FieldFactory.CreateModifiedField(
                            DateTools.DateToString(file.LastWriteTime, FieldFactory.ModifiedResolution)));

                    // Add the document to the index with the appropriate analyzer
                    indexWriter.AddDocument(document, parsingService.Analyzer);

                    // Add the file to the repository of indexed files
                    // It may already contain it if it's been updated
                    if(!repository.Contains(file))
                    {
                        repository.Add(file);
                        addedFiles++;
                    }
                    else
                        updatedFiles++;

                    // If a parser has been able to parse the file stop iterating through parsers
                    break;
                }
        }
    }
}
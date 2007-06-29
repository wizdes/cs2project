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
        private readonly string[] exclusions;
        private readonly IDictionary<string, FileInfo> filesWaitingToBeIndexed;

        private readonly Directory indexDirectory;
        private readonly IParsingService[] parsingServices;
        private readonly IFilesRepository repository;
        private readonly object updatingLock = new object();
        private int addedFilesSinceLastUpdate;
        private int deletedFilesSinceLastUpdate;
        private IndexReader indexReader;
        private IndexWriter indexWriter;
        private bool isUpdating = false;
        private int updatedFilesSinceLastUpdate;

        public IndexingService(Directory indexDirectory, IParsingService[] parsingServices, IFilesRepository repository,
                               string[] exclusions)
        {
            filesWaitingToBeIndexed = new SortedDictionary<string, FileInfo>();
            this.indexDirectory = indexDirectory;
            this.parsingServices = parsingServices;
            this.exclusions = exclusions;
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

        public int DeletedFilesSinceLastUpdate
        {
            get { return deletedFilesSinceLastUpdate; }
        }

        public int UpdatedFilesSinceLastUpdate
        {
            get { return updatedFilesSinceLastUpdate; }
        }

        public int AddedFilesSinceLastUpdate
        {
            get { return addedFilesSinceLastUpdate; }
        }

        public bool IsWaitingForFilesToBeIndexed
        {
            get { return filesWaitingToBeIndexed.Count != 0; }
        }

        public Directory IndexDirectory
        {
            get { return indexDirectory; }
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
            if(!filesWaitingToBeIndexed.ContainsKey(file.FullName))
                filesWaitingToBeIndexed.Add(file.FullName, file);
        }

        public void RequestIndexing(DirectoryInfo directory, SearchOption searchOption, string searchPattern)
        {
            if(!IsValidFileSystemEntryToBeIndexed(directory))
                return;

            foreach(FileInfo file in directory.GetFiles(searchPattern, searchOption))
                RequestIndexing(file);
        }

        public void RequestIndexing(DirectoryInfo directory, SearchOption searchOption, IProgrammingLanguage language)
        {
            RequestIndexing(directory, searchOption, language.SearchPattern);
        }

        public void RequestIndexing(DirectoryInfo directory, SearchOption searchOption)
        {
            RequestIndexing(directory, searchOption, "*");
        }

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

        public event EventHandler<IndexingCompletedEventArgs> IndexingCompleted;

        #endregion

        private bool IsValidFileSystemEntryToBeIndexed(FileSystemInfo entry)
        {
            // The file or directory is hidden
            if(entry.Attributes == FileAttributes.Hidden)
                return false;

            // The entry matches one of the exclusions
            foreach(string exclusion in exclusions)
            {
                Regex r = new Regex(Regex.Escape(exclusion));

                if(r.IsMatch(entry.FullName))
                    return false;
            }

            // The entry is a file which is already indexed (if it's out of date it will be automatically updated 
            // during the update process, no need to request it to be indexed again)
            if(entry is FileInfo && repository.Contains(entry as FileInfo))
                return false;

            return true;
        }

        private void OnIndexingCompleted()
        {
            if(IndexingCompleted != null)
                IndexingCompleted(this,
                                  new IndexingCompletedEventArgs(addedFilesSinceLastUpdate, updatedFilesSinceLastUpdate,
                                                                 deletedFilesSinceLastUpdate));
        }

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
            TermEnum idEnumerator = indexReader.Terms(new Term(FieldFactory.IDFieldName, ""));

            // Remove files no longer existing or out of date from the index
            while(idEnumerator.Term() != null && idEnumerator.Term().Field() == FieldFactory.IDFieldName)
            {
                string id = idEnumerator.Term().Text();

                // If file doesn't exist or if file exists but is out of date
                if(!File.Exists(IDIdentifierUtils.GetPathFromIdentifier(id)) || FileExistsButNeedsUpdating(id))
                {
                    // The delete document from the index
                    indexReader.DeleteDocuments(idEnumerator.Term());

                    // If file was deleted since out of date then re-index it
                    if(FileExistsButNeedsUpdating(id))
                    {
                        FileInfo file = new FileInfo(IDIdentifierUtils.GetPathFromIdentifier(id));

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

        private static bool FileExistsButNeedsUpdating(string id)
        {
            string path = IDIdentifierUtils.GetPathFromIdentifier(id);

            return File.Exists(path) && IDIdentifierUtils.GetIdentifierFromFile(new FileInfo(path)) != id;
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
                if(parsingService.TryParse(file, out document))
                {
                    document.Add(FieldFactory.CreateIDField(IDIdentifierUtils.GetIdentifierFromFile(file)));
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

    public class IndexingCompletedEventArgs : EventArgs
    {
        private readonly int addedFiles;
        private readonly int deletedFiles;
        private readonly int updatedFiles;

        public IndexingCompletedEventArgs(int addedFiles, int updatedFiles, int deletedFiles)
        {
            this.addedFiles = addedFiles;
            this.updatedFiles = updatedFiles;
            this.deletedFiles = deletedFiles;
        }

        public int Addedfiles
        {
            get { return addedFiles; }
        }

        public int UpdatedFiles
        {
            get { return updatedFiles; }
        }

        public int DeletedFiles
        {
            get { return deletedFiles; }
        }
    }
}
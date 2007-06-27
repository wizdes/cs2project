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
        private readonly IDictionary<string, FileInfo> filesWaitingToBeIndexed = new SortedList<string, FileInfo>();

        private readonly Directory indexDirectory;
        private readonly IParsingService[] parsingServices;
        private readonly IFilesRepository repository;
        private readonly object updatingLock = new object();
        private int addedFiles;
        private int deletedFiles;
        private IndexReader indexReader;
        private IndexWriter indexWriter;
        private bool isUpdating = false;
        private int updatedFiles;

        public IndexingService(Directory indexDirectory, IParsingService[] parsingServices, IFilesRepository repository,
                               string[] exclusions
            )
        {
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

        public int DeletedFiles
        {
            get { return deletedFiles; }
        }

        public int UpdatedFiles
        {
            get { return updatedFiles; }
        }

        public int AddedFiles
        {
            get { return addedFiles; }
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

            if(!filesWaitingToBeIndexed.ContainsKey(file.FullName))
                filesWaitingToBeIndexed.Add(file.FullName, file);
        }

        /// <summary>
        /// Triggers update operations on the index and on the files repository, removing no longer existing files references both from repository and index, updating changed documents and adding new documents which have been explicitly required to be indexed.
        /// </summary>
        public void UpdateIndex()
        {
            lock(updatingLock)
            {
                if(isUpdating)
                    return;

                isUpdating = true;
            }

            addedFiles = 0;
            updatedFiles = 0;
            deletedFiles = 0;

            // Create new IndexReader to update the index
            indexReader = IndexReader.Open(indexDirectory);

            RemoveDeletedFilesFromRepository();

            RemoveDeletedAndModifiedDocumentsFromIndex();

            // Close the IndexReader
            indexReader.Close();
            indexReader = null;

            // Create a new IndexWriter to add new documents to the index
            indexWriter = new IndexWriter(indexDirectory, new StandardAnalyzer(), false);

            AddNewAndUpdatedFilesToTheIndex();

            // Close the IndexWriter
            indexWriter.Optimize();
            indexWriter.Close();
            indexWriter = null;

            // Fire IndexingCompletedEvent
            OnIndexingCompleted();

            lock(updatingLock)
                isUpdating = false;
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

            // The entry is already indexed (if it's out of date will be automatically updated)
            if(entry is FileInfo && repository.Contains(entry as FileInfo))
                return false;

            return true;
        }

        private event EventHandler IndexingCompleted;

        private void OnIndexingCompleted()
        {
            if(IndexingCompleted != null)
                IndexingCompleted(this, EventArgs.Empty);
        }

        private void AddNewAndUpdatedFilesToTheIndex()
        {
            FileInfo[] filesToBeIndexed = new FileInfo[filesWaitingToBeIndexed.Count];
            filesWaitingToBeIndexed.Values.CopyTo(filesToBeIndexed, 0);

            foreach(FileInfo info in filesToBeIndexed)
                Index(info);
        }

        /// <summary>
        /// Removes the deleted and modified documents from the index. Marks the modified files as to be reindexed.
        /// </summary>
        private void RemoveDeletedAndModifiedDocumentsFromIndex()
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

                        if(!filesWaitingToBeIndexed.ContainsKey(file.FullName))
                            filesWaitingToBeIndexed.Add(file.FullName, file);
                    }
                }

                idEnumerator.Next();
            }

            idEnumerator.Close(); // close uid iterator
        }

        /// <summary>
        /// Removes the deleted files from repository.
        /// </summary>
        private void RemoveDeletedFilesFromRepository()
        {
            // TODO: is it right to remove items while iterating through it?
            foreach(string file in repository.GetAll())
                if(!File.Exists(file))
                {
                    repository.Remove(new FileInfo(file));
                    deletedFiles++;
                }
        }

        private static bool FileExistsButNeedsUpdating(string id)
        {
            return File.Exists(IDIdentifierUtils.GetPathFromIdentifier(id)) &&
                   IDIdentifierUtils.GetIdentifierFromFile(new FileInfo(IDIdentifierUtils.GetPathFromIdentifier(id))) !=
                   id;
        }

        /// <summary>
        /// Indexes the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        private void Index(FileInfo file)
        {
            Document document;

            // Find a parser that suites the file
            foreach(IParsingService parsingService in parsingServices)
            {
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
                    else updatedFiles++;

                    // If a parser has been able to parse the file stop iterating through parsers
                    break;
                }
            }

            filesWaitingToBeIndexed.Remove(file.FullName);
        }
    }
}
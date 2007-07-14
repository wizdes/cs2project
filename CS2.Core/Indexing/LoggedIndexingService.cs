using System;
using System.IO;
using Castle.Core.Logging;
using CS2.Core.Indexing;
using CS2.Core.Logging;
using Directory=Lucene.Net.Store.Directory;

namespace CS2.Core.Indexing
{
    public class LoggedIndexingService : IIndexingService, ILoggingService
    {
        private readonly IIndexingService inner;
        private ILogger logger = NullLogger.Instance;

        public LoggedIndexingService(IIndexingService inner)
        {
            logger.Info("IndexingService instantiated");
            this.inner = inner;
        }

        #region IIndexingService Members

        public int DeletedFilesSinceLastUpdate
        {
            get { return inner.DeletedFilesSinceLastUpdate; }
        }

        public int AddedFilesSinceLastUpdate
        {
            get { return inner.AddedFilesSinceLastUpdate; }
        }

        /// <summary>
        /// The directory where the index is located.
        /// </summary>
        public Directory IndexDirectory
        {
            get { return inner.IndexDirectory; }
        }

        /// <summary>
        /// Returns true if there are files waiting to be indexed in batch.
        /// </summary>
        /// <value></value>
        public bool IsWaitingForFilesToBeIndexed
        {
            get { return inner.IsWaitingForFilesToBeIndexed; }
        }

        /// <summary>
        /// Requests the indexing of the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void RequestIndexing(FileInfo file)
        {
            logger.InfoFormat("Requested indexing file {0}", file.FullName);
            inner.RequestIndexing(file);
        }

        public void RequestIndexing(DirectoryInfo directory)
        {
            logger.InfoFormat("Requested indexing directory {0}", directory.FullName);
            inner.RequestIndexing(directory);
        }

        /// <summary>
        /// Triggers update operations on the index, removing no longer existing documents, updating changed documents and adding new documents which have been explicitly required to be indexed.
        /// </summary>
        public void UpdateIndex()
        {
            logger.Info("Start updating index");
            inner.UpdateIndex();
            logger.Info("Finished updating index.");
            logger.InfoFormat("Files added: {0}", AddedFilesSinceLastUpdate);
            logger.InfoFormat("Files deleted: {0}", DeletedFilesSinceLastUpdate);
        }

        public event EventHandler<IndexingCompletedEventArgs> IndexingCompleted
        {
            add { inner.IndexingCompleted += value; }
            remove { inner.IndexingCompleted -= value; }
        }

        public string[] Exclusions
        {
            get { return inner.Exclusions; }
            set { inner.Exclusions = value; }
        }

        #endregion

        #region ILoggingService Members

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        #endregion
    }
}
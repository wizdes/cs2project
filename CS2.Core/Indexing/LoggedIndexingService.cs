using System;
using System.Diagnostics;
using System.IO;
using Castle.Core.Logging;
using CS2.Core.Logging;
using CS2.Core.Parsing;
using Directory=Lucene.Net.Store.Directory;

namespace CS2.Core.Indexing
{
    public class LoggedIndexingService : IIndexingService, ILoggingService
    {
        private readonly IIndexingService inner;
        private ILogger logger = NullLogger.Instance;

        public LoggedIndexingService(IIndexingService inner)
        {
            Trace.TraceInformation("IndexingService instantiated");
            this.inner = inner;
            inner.IndexingCompleted += inner_IndexingCompleted;
        }

        #region IIndexingService Members

        public IParsingService[] ParsingServices
        {
            get { return inner.ParsingServices; }
        }

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
            Trace.TraceInformation("Requested indexing file {0}", file.FullName);
            inner.RequestIndexing(file);
        }

        public void RequestIndexing(DirectoryInfo directory)
        {
            Trace.TraceInformation("Requested indexing directory {0}", directory.FullName);
            inner.RequestIndexing(directory);
        }

        /// <summary>
        /// Triggers update operations on the index, removing no longer existing documents, updating changed documents and adding new documents which have been explicitly required to be indexed.
        /// </summary>
        public void UpdateIndex()
        {
            Trace.TraceInformation("Call to UpdateIndex()");
            inner.UpdateIndex();
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

        private void inner_IndexingCompleted(object sender, IndexingCompletedEventArgs e)
        {
            Trace.TraceInformation("Update completed. Files added: {0}, files deleted: {1}", AddedFilesSinceLastUpdate,
                                   DeletedFilesSinceLastUpdate);
        }
    }
}
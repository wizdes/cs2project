using System.IO;
using Castle.Core.Logging;
using CS2.Model;
using CS2.Services.Indexing;
using Directory=Lucene.Net.Store.Directory;

namespace CS2.Services.Logging
{
    public class LoggingIndexingDecorator : ILoggingService, IIndexingService
    {
        private readonly IIndexingService inner;
        private ILogger logger = NullLogger.Instance;

        public LoggingIndexingDecorator(IIndexingService inner)
        {
            this.inner = inner;
        }

        #region IIndexingService Members

        /// <summary>
        /// The directory where the index is located.
        /// </summary>
        public Directory IndexDirectory
        {
            get { return inner.IndexDirectory; }
        }

        public bool IsWaitingForFilesToBeIndexed
        {
            get { return inner.IsWaitingForFilesToBeIndexed; }
        }

        public void RequestIndexing(FileInfo file)
        {
            logger.InfoFormat("Requested indexing file {0}", file.FullName);
            inner.RequestIndexing(file);
        }

        /// <summary>
        /// Requests the indexing of the specified directory, optionally using recursion and looking for files which match the supplied pattern.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="searchOption">The search option.</param>
        /// <param name="searchPattern">The search pattern.</param>
        public void RequestIndexing(DirectoryInfo directory, SearchOption searchOption, string searchPattern)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Requests the indexing of the specified directory, optionally using recursion and looking for files of the specified language.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="searchOption">The search option.</param>
        /// <param name="language">The language.</param>
        public void RequestIndexing(DirectoryInfo directory, SearchOption searchOption, IProgrammingLanguage language)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Requests the indexing of all the files contained in the specified directory, optionally using recursion.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="searchOption">The search option.</param>
        public void RequestIndexing(DirectoryInfo directory, SearchOption searchOption)
        {
            throw new System.NotImplementedException();
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
            inner.UpdateIndex();
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
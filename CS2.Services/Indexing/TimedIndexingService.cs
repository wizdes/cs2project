using System;
using System.IO;
using System.Threading;
using CS2.Model;
using Directory=Lucene.Net.Store.Directory;

namespace CS2.Services.Indexing
{
    public class TimedIndexingService : IIndexingService, IDisposable
    {
        private readonly IIndexingService inner;
        private readonly Timer timer;

        public string[] Exclusions
        {
            get { return inner.Exclusions; }
            set { inner.Exclusions = value; }
        }

        public TimedIndexingService(IIndexingService inner, TimeSpan updateInterval)
        {
            this.inner = inner;
            timer = new Timer(UpdateIndex, null, new TimeSpan(0, 0, 30), updateInterval);
        }

        #region IDisposable Members

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(false);
        }

        #endregion

        #region IIndexingService Members

        /// <summary>
        /// Requests the indexing of the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void RequestIndexing(FileInfo file)
        {
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
            inner.RequestIndexing(directory, searchOption, searchPattern);
        }

        /// <summary>
        /// Requests the indexing of the specified directory, optionally using recursion and looking for files of the specified language.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="searchOption">The search option.</param>
        /// <param name="language">The language.</param>
        public void RequestIndexing(DirectoryInfo directory, SearchOption searchOption, IProgrammingLanguage language)
        {
            inner.RequestIndexing(directory, searchOption, language);
        }

        /// <summary>
        /// Requests the indexing of all the files contained in the specified directory, optionally using recursion.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="searchOption">The search option.</param>
        public void RequestIndexing(DirectoryInfo directory, SearchOption searchOption)
        {
            inner.RequestIndexing(directory, searchOption);
        }

        /// <summary>
        /// Requests the indexing of all the files contained in the specified directory and all its subdirectories.
        /// </summary>
        /// <param name="directory">The directory.</param>
        public void RequestIndexing(DirectoryInfo directory)
        {
            inner.RequestIndexing(directory);
        }

        /// <summary>
        /// Triggers update operations on the index, removing no longer existing documents, updating changed documents and adding new documents which have been explicitly required to be indexed.
        /// </summary>
        public void UpdateIndex()
        {
            inner.UpdateIndex();
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
        public bool IsWaitingForFilesToBeIndexed
        {
            get { return inner.IsWaitingForFilesToBeIndexed; }
        }

        public int DeletedFilesSinceLastUpdate
        {
            get { return inner.DeletedFilesSinceLastUpdate; }
        }

        public int UpdatedFilesSinceLastUpdate
        {
            get { return inner.UpdatedFilesSinceLastUpdate; }
        }

        public int AddedFilesSinceLastUpdate
        {
            get { return inner.AddedFilesSinceLastUpdate; }
        }

        public event EventHandler<IndexingCompletedEventArgs> IndexingCompleted
        {
            add { inner.IndexingCompleted += value; }
            remove { inner.IndexingCompleted -= value; }
        }

        #endregion

        protected virtual void Dispose(bool isDisposing)
        {
            timer.Dispose();
        }

        private void UpdateIndex(object data)
        {
            UpdateIndex();
        }
    }
}
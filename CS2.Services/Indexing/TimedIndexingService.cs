using System;
using System.IO;
using System.Threading;
using CS2.Model;
using Directory=Lucene.Net.Store.Directory;

namespace CS2.Services.Indexing
{
    public class TimedIndexingService : IIndexingService
    {
        private readonly IIndexingService inner;
        private readonly Timer timer;

        public TimedIndexingService(IIndexingService inner)
        {
            this.inner = inner;
            timer = new Timer(UpdateIndex, null, new TimeSpan(0, 0, 30), new TimeSpan(0, 0, 30));
        }

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

        public int LastDeletedFiles
        {
            get { return inner.LastDeletedFiles; }
        }

        public int LastUpdatedFiles
        {
            get { return inner.LastUpdatedFiles; }
        }

        public int LastAddedFiles
        {
            get { return inner.LastAddedFiles; }
        }

        public event EventHandler IndexingCompleted
        {
            add { inner.IndexingCompleted += value; }
            remove { inner.IndexingCompleted -= value; }
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
           timer.Dispose();
        }

        #endregion

        private void UpdateIndex(object data)
        {
            UpdateIndex();
        }
    }
}
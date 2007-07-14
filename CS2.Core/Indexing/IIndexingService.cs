using System;
using System.IO;
using Directory=Lucene.Net.Store.Directory;

namespace CS2.Core.Indexing
{
    public interface IIndexingService
    {
        /// <summary>
        /// The directory where the index is located.
        /// </summary>
        Directory IndexDirectory { get; }

        /// <summary>
        /// Returns true if there are files waiting to be indexed in batch.
        /// </summary>
        bool IsWaitingForFilesToBeIndexed { get; }

        /// <summary>
        /// Returns the number of files deleted from the index since last update.
        /// </summary>
        int DeletedFilesSinceLastUpdate { get; }

        /// <summary>
        /// Returns the number of files added to the index since last update.
        /// </summary>
        int AddedFilesSinceLastUpdate { get; }

        string[] Exclusions { get; set; }

        /// <summary>
        /// Occurs when indexing is completed.
        /// </summary>
        event EventHandler<IndexingCompletedEventArgs> IndexingCompleted;

        /// <summary>
        /// Requests the indexing of the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        void RequestIndexing(FileInfo file);

        /// <summary>
        /// Requests the indexing of all the files contained in the specified directory and all its subdirectories.
        /// </summary>
        /// <param name="directory">The directory.</param>
        void RequestIndexing(DirectoryInfo directory);

        /// <summary>
        /// Triggers update operations on the index, removing no longer existing documents, updating changed documents and adding new documents which have been explicitly required to be indexed.
        /// </summary>
        void UpdateIndex();
    }
}
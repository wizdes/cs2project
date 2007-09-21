using System;

namespace CS2.Core.Indexing
{
    public class IndexingCompletedEventArgs : EventArgs
    {
        private readonly int addedFiles;
        private readonly int deletedFiles;
        private readonly int documentCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexingCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="addedFiles">The added files.</param>
        /// <param name="deletedFiles">The deleted files.</param>
        /// <param name="documentCount">The document count.</param>
        public IndexingCompletedEventArgs(int addedFiles, int deletedFiles, int documentCount)
        {
            this.addedFiles = addedFiles;
            this.deletedFiles = deletedFiles;
            this.documentCount = documentCount;
        }

        /// <summary>
        /// Gets the total number of documents contained in the index.
        /// </summary>
        /// <value>The document count.</value>
        public int DocumentCount
        {
            get { return documentCount; }
        }

        /// <summary>
        /// Gets the number of files added in the last mainteinance operation.
        /// </summary>
        /// <value>The added files.</value>
        public int AddedFiles
        {
            get { return addedFiles; }
        }

        /// <summary>
        /// Gets the number of files deleted in the last mainteinance operation.
        /// </summary>
        /// <value>The deleted files.</value>
        public int DeletedFiles
        {
            get { return deletedFiles; }
        }
    }
}
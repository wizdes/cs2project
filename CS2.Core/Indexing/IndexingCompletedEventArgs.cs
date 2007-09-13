using System;

namespace CS2.Core.Indexing
{
    public class IndexingCompletedEventArgs : EventArgs
    {
        private readonly int addedFiles;
        private readonly int deletedFiles;
        private readonly int documentCount;

        public IndexingCompletedEventArgs(int addedFiles, int deletedFiles, int documentCount)
        {
            this.addedFiles = addedFiles;
            this.deletedFiles = deletedFiles;
            this.documentCount = documentCount;
        }

        public int DocumentCount
        {
            get { return documentCount; }
        }

        public int AddedFiles
        {
            get { return addedFiles; }
        }

        public int DeletedFiles
        {
            get { return deletedFiles; }
        }
    }
}
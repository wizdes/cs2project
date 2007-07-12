using System;

namespace CS2.Services.Indexing
{
    public class IndexingCompletedEventArgs : EventArgs
    {
        private readonly int addedFiles;
        private readonly int deletedFiles;

        public IndexingCompletedEventArgs(int addedFiles, int deletedFiles)
        {
            this.addedFiles = addedFiles;
            this.deletedFiles = deletedFiles;
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
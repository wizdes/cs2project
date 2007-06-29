using System;

namespace CS2.Services.Indexing
{
    public class IndexingCompletedEventArgs : EventArgs
    {
        private readonly int addedFiles;
        private readonly int deletedFiles;
        private readonly int updatedFiles;

        public IndexingCompletedEventArgs(int addedFiles, int updatedFiles, int deletedFiles)
        {
            this.addedFiles = addedFiles;
            this.updatedFiles = updatedFiles;
            this.deletedFiles = deletedFiles;
        }

        public int AddedFiles
        {
            get { return addedFiles; }
        }

        public int UpdatedFiles
        {
            get { return updatedFiles; }
        }

        public int DeletedFiles
        {
            get { return deletedFiles; }
        }
    }
}
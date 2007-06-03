using System;

namespace CS2.Core.Models
{
    public class FileSystemSourceCodeRepository : ISourceCodeRepository
    {
        private Uri path;

        public Uri Path
        {
            get { return path; }
            set { path = value; }
        }
    }
}

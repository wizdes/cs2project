using System.IO;
using Lucene.Net.Store;
using Directory=Lucene.Net.Store.Directory;

namespace CS2.Services
{
    public static class DirectoryFactory
    {
        public static Directory GetDirectory(string path, bool overwrite)
        {
            if(!overwrite && !System.IO.Directory.Exists(path))
                throw new DirectoryNotFoundException(string.Format("The path {0} doesn't exist.", path));

            return FSDirectory.GetDirectory(path, overwrite);
        }
    }
}
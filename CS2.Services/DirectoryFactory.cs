using Lucene.Net.Store;

namespace CS2.Services
{
    /// <summary>
    /// Factory to create directories on the file system.
    /// </summary>
    public class DirectoryFactory
    {
        /// <summary>
        /// Returns a <see cref="Lucene.Net.Store.Directory" /> out of the specified path. If the directory doesn't exist it is created.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="overwrite">If set to <c>true</c> and the directory already exists it is overwritten.</param>
        /// <returns></returns>
        public Directory GetDirectory(string path, bool overwrite)
        {
            if(!System.IO.Directory.Exists(path))
                overwrite = true;

            return FSDirectory.GetDirectory(path, overwrite);
        }
    }
}
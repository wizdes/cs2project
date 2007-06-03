using System.IO;

namespace CS2.Core.Services.Indexing
{
    public interface IIndexingService
    {
        void Index(FileInfo localPath);
    }
}

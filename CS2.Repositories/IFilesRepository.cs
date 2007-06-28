using System.IO;

namespace CS2.Repositories
{
    public interface IFilesRepository
    {
        void Add(FileInfo file);
        void Remove(FileInfo file);
        bool Contains(FileInfo file);
        string[] GetAll();
    }
}
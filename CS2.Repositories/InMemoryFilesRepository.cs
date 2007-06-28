using System;
using System.Collections.Generic;
using System.IO;

namespace CS2.Repositories
{
    public class InMemoryFilesRepository : IFilesRepository
    {
        private readonly SortedList<string, string> files = new SortedList<string, string>();

        #region IFilesRepository Members

        public void Add(FileInfo file)
        {
            files.Add(file.FullName, file.FullName);
        }

        public void Remove(FileInfo file)
        {
            files.Remove(file.FullName);
        }

        public bool Contains(FileInfo file)
        {
            return files.ContainsKey(file.FullName);
        }

        public string[] GetAll()
        {
            return new List<string>(files.Keys).ToArray();
        }

        #endregion
    }
}
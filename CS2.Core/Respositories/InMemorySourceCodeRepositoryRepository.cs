using System;
using System.Collections.Generic;
using CS2.Core.Models;

namespace CS2.Core.Respositories
{
    class InMemorySourceCodeRepositoryRepository : ISourceCodeRepositoryRepository
    {
        private IDictionary<Uri, ISourceCodeRepository> storage = new Dictionary<Uri, ISourceCodeRepository>();

        public void Create(ISourceCodeRepository item)
        {
            storage.Add(item.Path, item);
        }

        public ISourceCodeRepository Find(Uri key)
        {
            return storage.ContainsKey(key) ? storage[key] : null;
        }

        ISourceCodeRepository[] ISourceCodeRepositoryRepository.FindAll()
        {
            return new List<ISourceCodeRepository>(storage.Values).ToArray();
        }

        public void Delete(ISourceCodeRepository item)
        {
            storage.Remove(item.Path);
        }

        public void Update(ISourceCodeRepository item)
        {
            if (Find(item.Path) != null)
                storage[item.Path] = item;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using CS2.Core.Models;

namespace CS2.Core.Respositories
{
    public interface ISourceCodeRepositoryRepository
    {
        void Create(ISourceCodeRepository item);

        ISourceCodeRepository Find(Uri key);

        ISourceCodeRepository[] FindAll();

        void Delete(ISourceCodeRepository item);

        void Update(ISourceCodeRepository identifiable);
    }
}

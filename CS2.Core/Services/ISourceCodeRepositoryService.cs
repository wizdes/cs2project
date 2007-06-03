using System;
using CS2.Core.Models;

namespace CS2.Core.Services
{
    public interface ISourceCodeRepositoryService
    {
        void IndexRepository(ISourceCodeRepository repository);
        bool IsRepositoryIndexed(ISourceCodeRepository repository);
        void RemoveRepository(ISourceCodeRepository repository);
    }
}
using System;
using CS2.Core.Models;
using CS2.Core.Respositories;
using CS2.Core.Services.Indexing;
using CS2.Core.Services.SourceControl;

namespace CS2.Core.Services
{
    public class CS2SourceCodeRepositoryService : ISourceCodeRepositoryService
    {
        private IIndexingService indexingService;
        private ISourceControlService[] sourceControlServices;
        private ISourceCodeRepositoryRepository sourceCodeRepositoryRepository;

        public CS2SourceCodeRepositoryService(  IIndexingService indexingService, 
                                                ISourceControlService[] sourceControlServices,
                                                ISourceCodeRepositoryRepository sourceCodeRepositoryRepository)
        {
            this.indexingService = indexingService;
            this.sourceControlServices = sourceControlServices;
            this.sourceCodeRepositoryRepository = sourceCodeRepositoryRepository;
        }

        public void IndexRepository(ISourceCodeRepository repository)
        {
            throw new NotImplementedException();
        }

        public bool IsRepositoryIndexed(ISourceCodeRepository repository)
        {
            throw new NotImplementedException();
        }

        public void RemoveRepository(ISourceCodeRepository repository)
        {
            throw new NotImplementedException();
        }
    }
}

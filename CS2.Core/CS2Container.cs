using System;
using System.Collections.Generic;
using System.Text;
using Castle.Windsor;
using CS2.Core.Services.Indexing;
using CS2.Core.Services.Parsing;
using CS2.Core.Models;
using CS2.Core.Respositories;
using CS2.Core.Services.SourceControl;
using CS2.Core.Services;

namespace CS2.Core
{
    class CS2Container : WindsorContainer
    {
        public CS2Container()
        {
            RegisterComponents();
        }

        private void RegisterComponents()
        {
            /* Models */
            
            AddComponent("Models.FileSystemSourceCodeRepository", typeof(ISourceCodeRepository), typeof(FileSystemSourceCodeRepository));
            AddComponent("Models.SubversionSourceCodeRepository", typeof(ISourceCodeRepository), typeof(SubversionSourceCodeRepository));

            /* Repositories */

            AddComponent("Repositories.InMemorySourceCodeRepositoryRepository", typeof(ISourceCodeRepositoryRepository), typeof(InMemorySourceCodeRepositoryRepository));

            /* Services */

            // Indexing
            AddComponent("Indexing.FileSystemIndexingService", typeof(IIndexingService), typeof(FileSystemIndexingService));

            // Parsing
            AddComponent("Parsing.CSharpSourceCodeParsingService", typeof(ISourceCodeParsingService), typeof(CSharpSourceCodeParsingService));

            // Source control
            AddComponent("SubversionSourceControlService", typeof(ISourceControlService), typeof(SubversionSourceControlService));

            // Main service
            AddComponent("CS2SourceCodeRepositoryService", typeof(ISourceCodeRepositoryService), typeof(CS2SourceCodeRepositoryService));
        }
    }
}

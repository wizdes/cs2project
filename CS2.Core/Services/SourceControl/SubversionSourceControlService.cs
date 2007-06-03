using System;

namespace CS2.Core.Services.SourceControl
{
    class SubversionSourceControlService : ISourceControlService
    {
        public void Checkout(Uri repositoryUri, Uri localPath)
        {
            throw new NotImplementedException();
        }

        public void CheckForModifications(Uri repositoryUri, Uri workingCopy)
        {
            throw new NotImplementedException();
        }

        public void Update(Uri repositoryUri, Uri workingCopy)
        {
            throw new NotImplementedException();
        }
    }
}

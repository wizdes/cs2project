using System;
using System.Collections.Generic;
using System.Text;
using CS2.Core.Models;

namespace CS2.Core.Services.SourceControl
{
    public interface ISourceControlService
    {
        void Checkout(ISourceCodeRepository repository, Uri localPath);
        void CheckForModifications(Uri repositoryUri, Uri workingCopy);
        void Update(Uri repositoryUri, Uri workingCopy);
    }
}

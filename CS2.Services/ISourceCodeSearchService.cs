using System;
using System.Collections.Generic;
using System.Text;
using CS2.Model;

namespace CS2.Services
{
    public interface ISourceCodeSearchService
    {
        IEnumerable<SourceCodeSearchResult> Search(string query);
    }
}

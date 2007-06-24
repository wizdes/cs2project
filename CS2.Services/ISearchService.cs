using System.Collections.Generic;
using CS2.Model;

namespace CS2.Services
{
    public interface ISearchService
    {
        IEnumerable<SearchResult> Search(SearchQuery query);
    }
}
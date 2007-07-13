using System.Collections.Generic;
using Lucene.Net.Documents;

namespace CS2.Services.Searching
{
    public interface ISearchService
    {
        IEnumerable<Document> Search(string query);
        IEnumerable<string> SearchWithHighlighting(string query);
    }
}
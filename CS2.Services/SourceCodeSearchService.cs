using System;
using System.Collections.Generic;
using System.Text;
using CS2.Model;
using Lucene.Net.Search;

namespace CS2.Services
{
    public class SourceCodeSearchService : ISourceCodeSearchService
    {
        private Searcher searcher;

        public SourceCodeSearchService(Searcher searcher)
        {
            this.searcher = searcher;
        }

        public IEnumerable<SourceCodeSearchResult> Search(string query)
        {
            return null;
        }
    }
}

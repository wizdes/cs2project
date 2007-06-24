using System;
using System.Collections.Generic;
using CS2.Model;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace CS2.Services
{
    public class SourceCodeSearchService : ISourceCodeSearchService
    {
        private readonly Searcher searcher;
        private readonly Analyzer analyzer;

        public SourceCodeSearchService(Searcher searcher, Analyzer analyzer)
        {
            this.searcher = searcher;
            this.analyzer = analyzer;
        }

        public IEnumerable<SearchResult> Search(SearchQuery query)
        {
            //Query q = new QueryParser().Parse(query.Query);

            //Hits hits = searcher.Search(q);

            //while(hits.Iterator().MoveNext())
            //{
            //    Hit hit = hits.Iterator().Current as Hit;

            //    yield return new SearchResult(hit);
            //}

            throw new NotImplementedException();
        }
    }
}

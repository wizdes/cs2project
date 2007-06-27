using System;
using System.Collections.Generic;
using CS2.Model;
using CS2.Services.Searching;
using Lucene.Net.Analysis;
using Lucene.Net.Search;

namespace CS2.Services.Searching
{
    public class SearchService
    {
        private readonly Searcher searcher;
        private readonly Analyzer analyzer;

        public SearchService(Searcher searcher, Analyzer analyzer)
        {
            this.searcher = searcher;
            this.analyzer = analyzer;
        }

//        public IEnumerable<SearchResult> Search(SearchQuery query)
//        {
//            //Query q = new QueryParser().TryParse(query.Query);
//
//            //Hits hits = searcher.Search(q);
//
//            //while(hits.Iterator().MoveNext())
//            //{
//            //    Hit hit = hits.Iterator().Current as Hit;
//
//            //    yield return new SearchResult(hit);
//            //}
//
//            throw new NotImplementedException();
//        }
    }
}
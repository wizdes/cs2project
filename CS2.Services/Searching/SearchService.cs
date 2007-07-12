using System.Collections.Generic;
using CS2.Services.Indexing;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace CS2.Services.Searching
{
    public class SearchService : ISearchService
    {
        private readonly IIndexingService indexingService;
        private IndexSearcher searcher;
        private readonly QueryParser queryParser;

        public SearchService(IIndexingService indexingService)
        {
            this.indexingService = indexingService;
            this.indexingService.IndexingCompleted += indexingService_IndexingCompleted;

            PerFieldAnalyzerWrapper analyzer = new PerFieldAnalyzerWrapper(new StandardAnalyzer());
            analyzer.AddAnalyzer("", new KeywordAnalyzer());
            queryParser = new QueryParser(FieldFactory.SourceFieldName, new StandardAnalyzer());

            searcher = new IndexSearcher(this.indexingService.IndexDirectory);
        }

        private void indexingService_IndexingCompleted(object sender, IndexingCompletedEventArgs e)
        {
            searcher = new IndexSearcher(indexingService.IndexDirectory);
        }

        public IEnumerable<Document> Search(string query)
        {
            Query q = queryParser.Parse(query);

            Hits hits = searcher.Search(q);

            for(int i = 0; i < hits.Length(); i++)
                yield return hits.Doc(i);
        }
    }
}
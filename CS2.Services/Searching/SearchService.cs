using System;
using System.Collections.Generic;
using System.IO;
using CS2.Services.Analysis;
using CS2.Services.Indexing;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Highlight;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace CS2.Services.Searching
{
    public class SearchService : ISearchService
    {
        private readonly IIndexingService indexingService;
        private readonly IDictionary<string, BaseAnalyzer> analyzers;
        private IndexSearcher searcher;
        private readonly Formatter formatter = new SimpleHTMLFormatter();

        public SearchService(IIndexingService indexingService, IDictionary<string, BaseAnalyzer> analyzers)
        {
            this.indexingService = indexingService;
            this.analyzers = analyzers;
            this.indexingService.IndexingCompleted += indexingService_IndexingCompleted;

            InstantiateSearcher();
        }

        private void indexingService_IndexingCompleted(object sender, IndexingCompletedEventArgs e)
        {
            InstantiateSearcher();
        }

        private void InstantiateSearcher()
        {
            searcher = new IndexSearcher(indexingService.IndexDirectory);
        }

        public IEnumerable<Document> Search(string query)
        {
            Query q = new TermQuery(new Term(FieldFactory.SourceFieldName, query));

            Hits hits = searcher.Search(q);

            for(int i = 0; i < hits.Length(); i++)
                yield return hits.Doc(i);
        }

        public IEnumerable<string> SearchWithHighlighting(string query)
        {
            IEnumerable<Document> docs = Search(query);

            Highlighter highlighter =
                new Highlighter(formatter, new QueryScorer(new TermQuery(new Term(FieldFactory.SourceFieldName, query))));
            highlighter.SetTextFragmenter(new SimpleFragmenter(50));

            foreach(Document doc in docs)
            {
                string path = doc.Get(FieldFactory.PathFieldName);

                TokenStream tokenStream =
                    analyzers[doc.Get(FieldFactory.LanguageFieldName)].TokenStream(FieldFactory.SourceFieldName,
                                                                                   new StreamReader(path));

                string[] fragments = highlighter.GetBestFragments(tokenStream, new StreamReader(path).ReadToEnd(), 10);

                foreach(string fragment in fragments)
                    yield return fragment;
            }
        }
    }
}
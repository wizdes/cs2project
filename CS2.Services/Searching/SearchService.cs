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
        private readonly IDictionary<string, AbstractAnalyzer> analyzers;
        private readonly IIndexingService indexingService;
        private Encoder encoder = new DefaultEncoder();
        private Formatter formatter = new SimpleHTMLFormatter();
        private Fragmenter fragmenter = new NullFragmenter();
        private IndexSearcher searcher;

        public SearchService(IIndexingService indexingService, IDictionary<string, AbstractAnalyzer> analyzers)
        {
            this.indexingService = indexingService;
            this.analyzers = analyzers;
            this.indexingService.IndexingCompleted += delegate { InstantiateSearcher(); };

            InstantiateSearcher();
        }

        #region ISearchService Members

        public Formatter Formatter
        {
            get { return formatter; }
            set { formatter = value; }
        }

        public Fragmenter Fragmenter
        {
            get { return fragmenter; }
            set { fragmenter = value; }
        }

        public Encoder Encoder
        {
            get { return encoder; }
            set { encoder = value; }
        }

        public IEnumerable<Document> Search(string query)
        {
            Query q = new TermQuery(new Term(FieldFactory.SourceFieldName, query));

            Hits hits = searcher.Search(q);

            for(int i = 0; i < hits.Length(); i++)
                yield return hits.Doc(i);
        }

        public IEnumerable<SearchResult> SearchWithHighlighting(string query)
        {
            IEnumerable<Document> docs = Search(query);

            Highlighter highlighter =
                new Highlighter(formatter, encoder, new QueryScorer(new TermQuery(new Term(FieldFactory.SourceFieldName, query))));
            highlighter.SetTextFragmenter(fragmenter);

            foreach(Document doc in docs)
            {
                string path = doc.Get(FieldFactory.PathFieldName);

                TokenStream tokenStream =
                    analyzers[doc.Get(FieldFactory.LanguageFieldName)].TokenStream(FieldFactory.SourceFieldName,
                                                                                   new StreamReader(path));

                string[] fragments = highlighter.GetBestFragments(tokenStream, new StreamReader(path).ReadToEnd(), 10);

                foreach(string fragment in fragments)
                    yield return new SearchResult(path, fragment);
            }
        }

        #endregion

        private void InstantiateSearcher()
        {
            searcher = new IndexSearcher(indexingService.IndexDirectory);
        }
    }
}
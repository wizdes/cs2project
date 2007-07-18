using System.Collections.Generic;
using System.IO;
using CS2.Core.Analysis;
using CS2.Core.Indexing;
using CS2.Core.Parsing;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Highlight;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace CS2.Core.Searching
{
    public class SearchService : ISearchService
    {
        #region Private fields

        private readonly IDictionary<string, AbstractAnalyzer> analyzers = new Dictionary<string, AbstractAnalyzer>();
        private readonly IIndexingService indexingService;
        private Encoder encoder = new DefaultEncoder();
        private Formatter formatter = new SimpleHTMLFormatter();
        private Fragmenter fragmenter = new NullFragmenter();
        private IndexSearcher searcher;

        #endregion

        public SearchService(IIndexingService indexingService)
        {
            this.indexingService = indexingService;
            this.indexingService.IndexingCompleted += delegate { InstantiateSearcher(); };

            foreach(IParsingService service in indexingService.ParsingServices)
                analyzers.Add(service.LanguageName, service.Analyzer);

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
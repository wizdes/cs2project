using System;
using System.Collections.Generic;
using System.IO;
using CS2.Core.Analysis;
using CS2.Core.Indexing;
using CS2.Core.Parsing;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Highlight;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
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
        private Fragmenter fragmenter = new SimpleFragmenter(50);
        private IndexSearcher searcher;

        #endregion

        public SearchService(IIndexingService indexingService)
        {
            this.indexingService = indexingService;

            // Refresh searcher each time the indexer has finished indexing
            this.indexingService.IndexingCompleted += delegate { searcher.Close(); InstantiateSearcher(); };

            // Create a list of all the analyzers, one for each language
            foreach (IParsingService service in indexingService.ParsingServices)
                analyzers.Add(service.LanguageName, service.Analyzer);

            // Instantiate the searcher for the fist time
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

            for (int i = 0; i < hits.Length(); i++)
                yield return hits.Doc(i);
        }

        public IEnumerable<SearchResult> SearchWithHighlighting(string query)
        {
            IEnumerable<Document> docs = Search(query);

            Highlighter highlighter =
                new Highlighter(formatter, encoder, new QueryScorer(new TermQuery(new Term(FieldFactory.SourceFieldName, query))));
            highlighter.SetTextFragmenter(fragmenter);

            foreach (Document doc in docs)
            {
                string path = doc.Get(FieldFactory.PathFieldName);

                TokenStream tokenStream =
                    analyzers[doc.Get(FieldFactory.LanguageFieldName)].TokenStream(FieldFactory.SourceFieldName,
                                                                                   new StreamReader(path));

                string[] fragments = highlighter.GetBestFragments(tokenStream, new StreamReader(path).ReadToEnd(), 10);

                foreach (string fragment in fragments)
                    yield return new SearchResult(doc, fragment);
            }
        }

        public IEnumerable<SearchResult> SearchWithQueryParser(string query)
        {
            if (string.IsNullOrEmpty(query))
                yield break;

            // Create a list of query parsers, one for each language, based on the language analyzer
            List<QueryParser> parsers = new List<QueryParser>(analyzers.Count);

            // See if the query contains specification for a particular language
            string language = GetLanguageFromQuery(query);

            PerFieldAnalyzerWrapper analyzerWrapper;

            // If no language was specified in the query or if there is no analyzer suitable for that language
            // then use all the analyzers to search the query
            // A KeywordAnalyzer is always used for the language field.
            if (string.IsNullOrEmpty(language) || !analyzers.ContainsKey(language))
                foreach (KeyValuePair<string, AbstractAnalyzer> analyzer in analyzers)
                {
                    analyzerWrapper = new PerFieldAnalyzerWrapper(analyzer.Value);
                    analyzerWrapper.AddAnalyzer(FieldFactory.LanguageFieldName, new KeywordAnalyzer());

                    parsers.Add(new QueryParser(FieldFactory.SourceFieldName, analyzerWrapper));
                }
            // Otherwise use just the analyzer corresponding to the specified language
            // A KeywordAnalyzer is always used for the language field.
            else
            {
                analyzerWrapper = new PerFieldAnalyzerWrapper(analyzers[language]);
                analyzerWrapper.AddAnalyzer(FieldFactory.LanguageFieldName, new KeywordAnalyzer());

                parsers.Add(new QueryParser(FieldFactory.SourceFieldName, analyzerWrapper));
            }

            foreach (QueryParser parser in parsers)
            {
                Query q = parser.Parse(query).Rewrite(searcher.GetIndexReader());

                Hits hits = searcher.Search(q);

                Highlighter highlighter = new Highlighter(formatter, encoder, new QueryScorer(q));
                highlighter.SetTextFragmenter(fragmenter);

                for (int i = 0; i < hits.Length(); i++)
                {
                    Document doc = hits.Doc(i);

                    string path = doc.Get(FieldFactory.PathFieldName);

                    TokenStream tokenStream =
                        parser.GetAnalyzer().TokenStream(FieldFactory.SourceFieldName, new StreamReader(path));

                    string[] fragments = highlighter.GetBestFragments(tokenStream, new StreamReader(path).ReadToEnd(), 10);

                    if (fragments.Length == 0)
                        using (StreamReader reader = new StreamReader(path))
                            fragments = new string[] { reader.ReadToEnd().Substring(0, 100) };

                    foreach (string fragment in fragments)
                        yield return new SearchResult(doc, fragment);
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets the language string from the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The language name in lower case if the query contains one, null otherwise.</returns>
        private static string GetLanguageFromQuery(string query)
        {
            if (!query.Contains(FieldFactory.LanguageFieldName))
                return null;

            List<string> terms = new List<string>(query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            return
                terms.Find(delegate(string s) { return s.Contains(FieldFactory.LanguageFieldName); }).Split(':')[1].
                    ToLowerInvariant();
        }

        private void InstantiateSearcher()
        {
            searcher = new IndexSearcher(indexingService.IndexDirectory);
        }
    }
}
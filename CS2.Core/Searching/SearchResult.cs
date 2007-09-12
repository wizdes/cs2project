using Lucene.Net.Documents;

namespace CS2.Core.Searching
{
    public class SearchResult
    {
        private readonly string filePath;
        private readonly string language;
        private readonly string snippet;

        public SearchResult(Document doc, string snippet)
        {
            filePath = doc.Get(FieldFactory.PathFieldName);
            language = doc.Get(FieldFactory.LanguageFieldName);
            this.snippet = snippet;
        }

        public string Language
        {
            get { return language; }
        }

        public string FilePath
        {
            get { return filePath; }
        }

        public string Snippet
        {
            get { return snippet; }
        }
    }
}
using Lucene.Net.Documents;

namespace CS2.Core.Searching
{
    public class SearchResult
    {
        private readonly string filePath;
        private readonly string language;
        private readonly string snippet;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResult"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="snippet">The snippet.</param>
        public SearchResult(Document doc, string snippet)
        {
            filePath = doc.Get(FieldFactory.PathFieldName);
            language = doc.Get(FieldFactory.LanguageFieldName);
            this.snippet = snippet;
        }

        /// <summary>
        /// Gets the programming language of the document.
        /// </summary>
        /// <value>The language.</value>
        public string Language
        {
            get { return language; }
        }

        /// <summary>
        /// Gets the path of the file referenced by the document.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath
        {
            get { return filePath; }
        }

        /// <summary>
        /// Gets the snippet of code where the search term has been found, with highlighting.
        /// </summary>
        /// <value>The snippet.</value>
        public string Snippet
        {
            get { return snippet; }
        }
    }
}
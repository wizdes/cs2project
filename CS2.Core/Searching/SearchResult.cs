namespace CS2.Core.Searching
{
    public class SearchResult
    {
        private readonly string filePath;
        private readonly string snippet;

        public string FilePath
        {
            get { return filePath; }
        }

        public string Snippet
        {
            get { return snippet; }
        }

        public SearchResult(string filePath, string snippet)
        {
            this.filePath = filePath;
            this.snippet = snippet;
        }
    }
}
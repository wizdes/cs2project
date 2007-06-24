using System;

namespace CS2.Model
{
    public class SearchQuery
    {
        private SearchScope searchScope;
        private string query;

        public SearchScope SearchScope
        {
            get { return searchScope; }
            set { searchScope = value; }
        }

        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        public bool IsMultipleFieldQuery
        {
            get
            {
                double log = Math.Log((double) searchScope, 2);
                return log == (int) log;
            }
        }

        public string[] Fields
        {
            get { return null; }
        }

        public SearchQuery(SearchScope searchScope, string query)
        {
            this.searchScope = searchScope;
            this.query = query;
        }

        public SearchQuery(string query) : this(SearchScope.Fulltext, query)
        {
        }
    }
}

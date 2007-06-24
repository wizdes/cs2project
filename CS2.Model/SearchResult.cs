using System;
using Lucene.Net.Search;

namespace CS2.Model
{
    public class SearchResult
    {
        private readonly Hit hit;

        public Hit Hit
        {
            get { return hit; }
        }

        public SearchResult(Hit hit)
        {
            this.hit = hit;
        }
    }
}

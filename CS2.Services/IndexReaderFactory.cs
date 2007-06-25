using Lucene.Net.Index;
using Lucene.Net.Store;

namespace CS2.Services
{
    public static class IndexReaderFactory
    {
        public static IndexReader Create(Directory directory)
        {
            return IndexReader.Open(directory);
        }
    }
}
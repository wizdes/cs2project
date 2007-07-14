using System.Collections;
using System.Collections.Specialized;
using CS2.Core.Searching;
using Lucene.Net.Search;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class SearchServiceTests : BaseTest
    {
        [Test]
        public void CanResolve()
        {
            Searcher searcher = mocks.CreateMock<Searcher>();

            IDictionary parameters = new ListDictionary();
            parameters.Add("searcher", searcher);

            ISearchService service = container.Resolve<ISearchService>(parameters);

            Assert.IsNotNull(service);
        }

        [Test]
        public void CanSearch()
        {
//            ISearchService service = mocks.CreateMock<ISearchService>();
//
//            List<SearchResult> list = new List<SearchResult>();
//            list.Add(new SearchResult(null));
//
//            SearchQuery query = new SearchQuery("test");
//            
//            Expect.Call(service.Search(query)).Return(list);
//
//            mocks.ReplayAll();
//
//            Assert.AreEqual(service.Search(query), list);
//
//            mocks.VerifyAll();
        }
    }
}

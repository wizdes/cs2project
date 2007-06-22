using System.Collections.Generic;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using CS2.Model;
using CS2.Services;
using Lucene.Net.Search;
using NUnit.Framework;
using Rhino.Mocks;

namespace CS2.Tests
{
    [TestFixture]
    public class SourceCodeSearchServiceTests
    {
        private IWindsorContainer container;
        private MockRepository mocks;


        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            container = new WindsorContainer(new XmlInterpreter());
            mocks = new MockRepository();
        }

        [Test]
        public void CanResolve()
        {
            Searcher searcher = mocks.CreateMock<Searcher>();

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("searcher", searcher);

            ISourceCodeSearchService service = container.Resolve<ISourceCodeSearchService>(parameters);

            Assert.IsNotNull(service);
        }

        [Test]
        public void CanSearch()
        {
            ISourceCodeSearchService service = mocks.CreateMock<ISourceCodeSearchService>();

            List<SourceCodeSearchResult> list = new List<SourceCodeSearchResult>();
            list.Add(new SourceCodeSearchResult());
            
            Expect.Call(service.Search("test")).Return(list);

            mocks.ReplayAll();

            Assert.AreEqual(service.Search("test"), list);

            mocks.VerifyAll();
        }
    }
}

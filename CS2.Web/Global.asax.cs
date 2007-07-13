using System;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using CS2.Services.Indexing;
using CS2.Services.Searching;

namespace CS2.Web
{
    public class Global : System.Web.HttpApplication
    {
        private static readonly IWindsorContainer container = new WindsorContainer(new XmlInterpreter());
        private static IIndexingService indexingService;
        private static ISearchService searchService;

        public static IIndexingService IndexingService
        {
            get { return indexingService; }
        }

        public static ISearchService SearchService
        {
            get { return searchService; }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            indexingService = container.Resolve<IIndexingService>();
            searchService = container.Resolve<ISearchService>();
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
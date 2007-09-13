using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using CS2.Core.Indexing;
using CS2.Core.Searching;

namespace CS2.Web
{
    public class Global : HttpApplication
    {
        private static readonly IWindsorContainer container = new WindsorContainer(new XmlInterpreter());
        private static IIndexingService indexingService;
        private static ISearchService searchService;

        public static IIndexingService IndexingService
        {
            get { return indexingService; }
        }

        public static IEnumerable<SearchResult> Search(string query, int maximumRows, int startRowIndex)
        {
            int count = Count(query);

            return
                GetAndCacheResults(query).GetRange(Math.Min(startRowIndex, count),
                                                   Math.Min(maximumRows, count - Math.Min(startRowIndex, count)));
        }

        public static int Count(string query)
        {
            return GetAndCacheResults(query).Count;
        }

        private static List<SearchResult> GetAndCacheResults(string query)
        {
            if(HttpContext.Current.Items["results"] == null)
            {
                Stopwatch watch = new Stopwatch();

                watch.Start();

                HttpContext.Current.Items["results"] = new List<SearchResult>(searchService.SearchWithQueryParser(query));

                watch.Stop();
                HttpContext.Current.Items["elapsed"] = watch.ElapsedMilliseconds;

                return HttpContext.Current.Items["results"] as List<SearchResult>;
            }
            return HttpContext.Current.Items["results"] as List<SearchResult>;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Start(object sender, EventArgs e)
        {
            indexingService = container.Resolve<IIndexingService>();
            searchService = container.Resolve<ISearchService>();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            container.Release(indexingService);
            container.Release(searchService);

            container.Dispose();
        }
    }
}
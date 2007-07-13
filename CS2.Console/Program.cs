using System;
using System.IO;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using CS2.Services.Indexing;
using CS2.Services.Searching;

namespace CS2.Console
{
    internal class Program
    {
        private static void Main()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter());

            IIndexingService indexingService = container.Resolve<IIndexingService>();
            ISearchService searchService = container.Resolve<ISearchService>();

            indexingService.RequestIndexing(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent);

            while(true)
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Type file/directory to index or search query or press enter to exit: ");
                string indexingRequestPath = System.Console.ReadLine();

                if(indexingRequestPath == string.Empty)
                {
                    System.Console.WriteLine("Exiting...");
                    break;
                }

                // Index file
                if(File.Exists(indexingRequestPath))
                    indexingService.RequestIndexing(new FileInfo(indexingRequestPath));
                // Index directory
                else if(Directory.Exists(indexingRequestPath))
                    indexingService.RequestIndexing(new DirectoryInfo(indexingRequestPath));
                // Search
                else
                {
                    System.Console.WriteLine("Search results:");

                    //foreach(Document document in searchService.Search(indexingRequestPath))
                    //    System.Console.WriteLine(document.Get(FieldFactory.PathFieldName));
                    foreach(string result in searchService.SearchWithHighlighting(indexingRequestPath))
                    {
                        System.Console.WriteLine(result);
                    }
                }
            }
        }
    }
}
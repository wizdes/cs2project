using System;
using System.Collections.Generic;
using System.IO;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using CS2.Core;
using CS2.Core.Indexing;
using CS2.Core.Searching;
using Lucene.Net.Documents;

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
                    IEnumerable<Document> searchResults = searchService.Search(indexingRequestPath);
                    System.Console.WriteLine("{0} matches found.", new List<Document>(searchResults).Count);

                    foreach(Document document in searchResults)
                        System.Console.WriteLine(document.Get(FieldFactory.PathFieldName));
                }
            }
        }
    }
}
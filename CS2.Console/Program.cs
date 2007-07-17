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

            string input;
            
            do
            {
                System.Console.WriteLine("Type file/directory to index or search query or press enter to exit:\n");

                if ((input = System.Console.ReadLine()) != string.Empty)
                {
                    // Index file
                    if(File.Exists(input))
                        indexingService.RequestIndexing(new FileInfo(input));
                        // Index directory
                    else if(Directory.Exists(input))
                        indexingService.RequestIndexing(new DirectoryInfo(input));
                        // Search
                    else
                    {
                        IEnumerable<Document> searchResults = searchService.Search(input);

                        System.Console.WriteLine("{0} matches found.", new List<Document>(searchResults).Count);

                        foreach(Document document in searchResults)
                            System.Console.WriteLine(document.Get(FieldFactory.PathFieldName));
                    }

                    System.Console.WriteLine();
                }

            } while(input != string.Empty);

            System.Console.WriteLine("Exiting...");
        }
    }
}
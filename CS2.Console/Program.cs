using System;
using System.IO;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using CS2.Services.Indexing;

namespace CS2.Console
{
    internal class Program
    {
        private static void Main()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter());

            IIndexingService indexingService = container.Resolve<IIndexingService>();

            // Can subscribe to IndexedCompleted event
            // indexingService.IndexingCompleted += indexingService_IndexingCompleted;

            indexingService.RequestIndexing(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent);

            while(true)
            {
                System.Console.WriteLine("Type file or directory path to index or press enter to exit: ");
                string indexingRequestPath = System.Console.ReadLine();

                if(indexingRequestPath == string.Empty)
                {
                    System.Console.WriteLine("Exiting...");
                    break;
                }

                if(File.Exists(indexingRequestPath))
                    indexingService.RequestIndexing(new FileInfo(indexingRequestPath));
                else if(Directory.Exists(indexingRequestPath))
                    indexingService.RequestIndexing(new DirectoryInfo(indexingRequestPath));
            }
        }
    }
}
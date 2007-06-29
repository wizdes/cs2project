using System;
using System.Diagnostics;
using System.IO;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using CS2.Services.Indexing;

namespace CS2.Console
{
    internal class Program
    {
        private const string docsDir = @"C:\Development\Rhino-tools\trunk\rhino-commons";
        private IIndexingService indexingService;

        private static void Main(string[] args)
        {
            new Program().Run();
        }

        private void indexingService_IndexingCompleted(object sender, IndexingCompletedEventArgs e)
        {
            Debug.WriteLine("Indexing completed");
            PrintFileOperations(e);
        }

        private static void PrintFileOperations(IndexingCompletedEventArgs e)
        {
            Debug.WriteLine("Added files: {0}" + e.Addedfiles);
            Debug.WriteLine("Updated files: {0}" + e.UpdatedFiles);
            Debug.WriteLine("Deleted files: {0}" + e.DeletedFiles);
        }

        private void Run()
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter());

            indexingService = container.Resolve<IIndexingService>();

            indexingService.IndexingCompleted += indexingService_IndexingCompleted;

            indexingService.RequestIndexing(new DirectoryInfo(docsDir));

            indexingService.RequestIndexing(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent);

            System.Console.ReadLine();
        }
    }
}
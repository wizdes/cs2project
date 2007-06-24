using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using CS2.Services;

namespace CS2.Console
{
    class Program
    {
        private const string docsDir = @"C:\Development\Rhino-tools\trunk";

        static void Main(string[] args)
        {
            IWindsorContainer container = new WindsorContainer(new XmlInterpreter());

            AbstractIndexingService indexingService = container.Resolve<AbstractIndexingService>();

            Debug.WriteLine(indexingService.IndexWriter.GetDirectory().GetType());

            indexingService.Index(new DirectoryInfo(docsDir));

            Debug.WriteLine(indexingService.IndexWriter.GetHashCode());

            indexingService.Index(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent);

            Debug.WriteLine(indexingService.IndexWriter.GetHashCode());

        }
    }
}

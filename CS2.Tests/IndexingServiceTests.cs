using System;
using System.IO;
using CS2.Services;
using NUnit.Framework;
using System.Diagnostics;

namespace CS2.Tests
{
    [TestFixture]
    public class IndexingServiceTests : BaseTest
    {
        private const string docsDir = @"C:\Development\Rhino-tools";

        [Test]
        public void CanResolve()
        {
            AbstractIndexingService indexingService = container.Resolve<AbstractIndexingService>();

            Assert.IsNotNull(indexingService);

            indexingService.IndexWriter.Close();
        }

        [Test]
        public void CanIndexDirectory()
        {
            AbstractIndexingService indexingService = container.Resolve<AbstractIndexingService>();

            Debug.WriteLine(indexingService.IndexWriter.GetDirectory().GetType());

            indexingService.Index(new DirectoryInfo(docsDir));

            Debug.WriteLine(indexingService.IndexWriter.GetHashCode());

            indexingService.Index(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent);

            Debug.WriteLine(indexingService.IndexWriter.GetHashCode());

            Assert.AreEqual(indexingService.IndexWriter.GetDirectory().List().Length, 3);            
        }

        //[Test]
        //public void CanIndexFile()
        //{
        //    AbstractIndexingService indexingService = container.Resolve<AbstractIndexingService>();
        //    indexingService.Index(new FileInfo(@"..\..\DummyClassForParseTesting.cs"));
        //}
    }
}
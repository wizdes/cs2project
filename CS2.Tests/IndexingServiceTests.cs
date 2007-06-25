using System;
using System.Diagnostics;
using System.IO;
using CS2.Services.Indexing;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class IndexingServiceTests : BaseTest
    {
        private const string docsDir = @"C:\Development\Rhino-tools\trunk\rhino-commons";

        [Test]
        public void CanResolve()
        {
            IIndexingService indexingService = container.Resolve<IIndexingService>();

            Assert.IsNotNull(indexingService);

            Debug.WriteLine(indexingService.GetType());
            Debug.WriteLine(indexingService.ParsingService.GetType());

            indexingService.IndexWriter.Close();
        }

        [Test]
        public void CanIndexDirectory()
        {
            IIndexingService indexingService = container.Resolve<IIndexingService>();

            Debug.WriteLine(indexingService.IndexWriter.GetDirectory().GetType());

            indexingService.Index(new DirectoryInfo(docsDir));

            Debug.WriteLine(indexingService.IndexWriter.GetHashCode());

            indexingService.Index(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent);

            Debug.WriteLine(indexingService.IndexWriter.GetHashCode());

            Assert.AreEqual(indexingService.IndexWriter.GetDirectory().List().Length, 3);
        }

        [Test]
        public void CanIndexFile()
        {
            IIndexingService indexingService = container.Resolve<IIndexingService>();
            indexingService.Index(new FileInfo(@"..\..\DummyClassForParseTesting.cs"));
        }
    }
}
using System.Diagnostics;
using System.IO;
using CS2.Services.Indexing;
using Lucene.Net.Index;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class IndexingServiceTests : BaseTest
    {
        private IIndexingService service;
        private const string docsDir = @"C:\Development\Rhino-tools\trunk\rhino-commons";

        [SetUp]
        public void Setup()
        {
            service = container.Resolve<IIndexingService>();
        }

        [Test]
        public void CanResolve()
        {
            Assert.IsNotNull(service);

            Debug.WriteLine(service.GetType());
        }

        [Test]
        public void IndexExists()
        {
            Assert.IsTrue(IndexReader.IndexExists(service.IndexDirectory));
        }

        [Test]
        public void HasFilesWaitingToBeIndexed()
        {
            Assert.IsFalse(service.IsWaitingForFilesToBeIndexed);

            service.RequestIndexing(new FileInfo(@"..\..\DummyClassForParseTesting.cs"));
            service.RequestIndexing(new DirectoryInfo(docsDir));

            Assert.IsTrue(service.IsWaitingForFilesToBeIndexed);
        }

        [Test]
        public void CanIndexFiles()
        {
            service.UpdateIndex();

            Assert.IsFalse(service.IsWaitingForFilesToBeIndexed);
        }
    }
}
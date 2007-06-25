using System;
using System.Diagnostics;
using System.IO;
using CS2.Services.Indexing;
using Lucene.Net.Index;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class IndexReaderTests : BaseTest
    {
        [SetUp]
        public void Setup()
        {
            // create the index
            IIndexingService indexingService = container.Resolve<IIndexingService>();

            indexingService.Index(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent);
        }

        [Test]
        public void CanResolve()
        {
            IndexReader reader = container.Resolve<IndexReader>();

            Assert.IsNotNull(reader);

            Debug.WriteLine("Number of documents: " + reader.NumDocs());
        }

        [Test]
        public void AreSame()
        {
            IndexReader reader1 = container.Resolve<IndexReader>();
            IndexReader reader2 = container.Resolve<IndexReader>();

            Assert.AreSame(reader1, reader2);
        }
    }
}
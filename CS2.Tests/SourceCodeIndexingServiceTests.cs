using System;
using System.IO;
using CS2.Services;
using NUnit.Framework;
using System.Diagnostics;

namespace CS2.Tests
{
    [TestFixture]
    public class SourceCodeIndexingServiceTests : BaseTest
    {
        [Test]
        public void CanResolve()
        {
            ISourceCodeIndexingService indexingService = container.Resolve<ISourceCodeIndexingService>();

            Assert.IsNotNull(indexingService);

            indexingService.IndexWriter.Close();
        }

        [Test]
        public void CanCreateIndex()
        {
            ISourceCodeIndexingService indexingService = container.Resolve<ISourceCodeIndexingService>();

            Debug.WriteLine(indexingService.IndexWriter.GetDirectory().GetType());

            indexingService.Index(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent);

            Debug.WriteLine(indexingService.IndexWriter.GetHashCode());

            indexingService.Index(new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent);

            Debug.WriteLine(indexingService.IndexWriter.GetHashCode());

            Assert.AreEqual(indexingService.IndexWriter.GetDirectory().List().Length, 3);            
        }
    }
}
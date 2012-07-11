using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CS2.CSharp.Parsing;
using CS2.Core;
using CS2.Core.Indexing;
using Lucene.Net.Index;
using Lucene.Net.Store;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class IndexingServiceTests : BaseTest
    {
        private IIndexingService service;
        private const string docsDir = @"..\..";
        private const string dummyFile = @"..\..\DummyClassForParseTesting.cs";

        [SetUp]
        public void Setup()
        {
            service = new LoggedIndexingService(new IndexingService(new RAMDirectory(), new[]{new CSharpParsingService()}, new LoggedSynchronizedStringSet(new SynchronizedStringSet())));
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

            service.RequestIndexing(new FileInfo(dummyFile));
            service.RequestIndexing(new DirectoryInfo(docsDir));

            Thread.Sleep(1000);

            Assert.IsTrue(service.IsWaitingForFilesToBeIndexed);
        }

        [Test]
        public void CanIndexFiles()
        {
            service.RequestIndexing(new FileInfo(dummyFile));
            service.UpdateIndex();

            PrintFileOperations();

            Assert.Greater(service.AddedFilesSinceLastUpdate, 0);
            Assert.AreEqual(0, service.DeletedFilesSinceLastUpdate);

            Assert.IsFalse(service.IsWaitingForFilesToBeIndexed);
        }

        [Test]
        public void ShoudlntUpdateAnything()
        {
            service.UpdateIndex();

            PrintFileOperations();

            Assert.AreEqual(0, service.AddedFilesSinceLastUpdate);
            Assert.AreEqual(0, service.DeletedFilesSinceLastUpdate);
        }

        [Test]
        public void ShouldntIndexAgainTheSameFiles()
        {
            service.RequestIndexing(new FileInfo(dummyFile));

            service.UpdateIndex();

            service.RequestIndexing(new FileInfo(dummyFile));

            service.UpdateIndex();

            PrintFileOperations();

            Assert.AreEqual(0, service.AddedFilesSinceLastUpdate);
            Assert.AreEqual(0, service.DeletedFilesSinceLastUpdate);
        }

        [Test]
        public void AreDifferentLastWriteTimes()
        {
            long ticks1 = File.GetLastWriteTime(dummyFile).Ticks;

            File.SetLastWriteTime(dummyFile, DateTime.Now);

            long ticks2 = File.GetLastWriteTime(dummyFile).Ticks;

            Assert.AreNotEqual(ticks1, ticks2);
        }

        [Test]
        public void ShouldReindexIfFileHasBeenModified()
        {
            service.RequestIndexing(new FileInfo(dummyFile));
            service.UpdateIndex();

            File.SetLastWriteTime(dummyFile, DateTime.Now);
            service.UpdateIndex();

            PrintFileOperations();

            Assert.AreEqual(1, service.AddedFilesSinceLastUpdate);
            Assert.AreEqual(1, service.DeletedFilesSinceLastUpdate);
        }

        private void PrintFileOperations()
        {
            Debug.WriteLine("Added files: " + service.AddedFilesSinceLastUpdate);
            Debug.WriteLine("Deleted files: " + service.DeletedFilesSinceLastUpdate);
        }
    }
}
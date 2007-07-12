using System;
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
        private const string dummyFile = @"..\..\DummyClassForParseTesting.cs";

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

            service.RequestIndexing(new FileInfo(dummyFile));
            service.RequestIndexing(new DirectoryInfo(docsDir));

            Assert.IsTrue(service.IsWaitingForFilesToBeIndexed);
        }

        [Test]
        public void CanIndexFiles()
        {
            service.UpdateIndex();

            PrintFileOperations();

            Assert.Greater(service.AddedFilesSinceLastUpdate, 0);
            Assert.AreEqual(service.DeletedFilesSinceLastUpdate, 0);

            Assert.IsFalse(service.IsWaitingForFilesToBeIndexed);
        }

        [Test]
        public void ShouldntIndexAgainTheSameFiles()
        {
            service.RequestIndexing(new FileInfo(dummyFile));
            service.UpdateIndex();

            PrintFileOperations();

            Assert.AreEqual(service.AddedFilesSinceLastUpdate, 0);
            Assert.AreEqual(service.DeletedFilesSinceLastUpdate, 0);
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
            File.SetLastWriteTime(dummyFile, DateTime.Now);
            service.UpdateIndex();

            PrintFileOperations();

            Assert.AreEqual(service.AddedFilesSinceLastUpdate, 0);
            Assert.AreEqual(service.DeletedFilesSinceLastUpdate, 0);
        }

        private void PrintFileOperations()
        {
            Debug.WriteLine("Added files: " + service.AddedFilesSinceLastUpdate);
            Debug.WriteLine("Deleted files: " + service.DeletedFilesSinceLastUpdate);
        }
    }
}
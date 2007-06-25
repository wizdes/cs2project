using System;
using System.IO;
using CS2.Services.Indexing;
using CS2.Services.Updating;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class UpdatingServiceTests : BaseTest
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            // create the index
            IIndexingService indexingService = container.Resolve<IIndexingService>();

            indexingService.Index(directory);
        }

        #endregion

        private readonly DirectoryInfo directory = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent;

        private FileInfo GetFirstSourceCodeFile()
        {
            return directory.GetFiles("DummyClassForParseTesting.cs",
                                      SearchOption.AllDirectories)[0];
        }

        [Test]
        public void CanResolve()
        {
            IUpdatingService updatingService = container.Resolve<IUpdatingService>();

            Assert.IsNotNull(updatingService);
        }

        [Test]
        public void IsRightNumberOfFiles()
        {
            IUpdatingService updatingService = container.Resolve<IUpdatingService>();

            Assert.AreEqual(updatingService.IndexReader.NumDocs(),
                            directory.GetFiles(container.Resolve<IIndexingService>().Language.FileExtension,
                                               SearchOption.AllDirectories).Length);
        }

        [Test]
        public void ShouldntNeedUpdating()
        {
            IUpdatingService updatingService = container.Resolve<IUpdatingService>();

            Assert.IsFalse(updatingService.NeedsUpdating);
        }

        [Test]
        public void ShouldNeedUpdating()
        {
            IUpdatingService updatingService = container.Resolve<IUpdatingService>();

            GetFirstSourceCodeFile().LastWriteTime = DateTime.Now;

            Assert.IsTrue(updatingService.NeedsUpdating);
        }
    }
}
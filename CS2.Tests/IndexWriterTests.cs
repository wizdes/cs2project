using Lucene.Net.Index;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class IndexWriterTests : BaseTest
    {
        [Test]
        public void CanResolve()
        {
            IndexWriter writer = container.Resolve<IndexWriter>();

            Assert.IsNotNull(writer);

            writer.Close();
        }

        [Test]
        public void DirectoryContainsFiles()
        {
            IndexWriter writer = container.Resolve<IndexWriter>();

            Assert.Greater(writer.GetDirectory().List().Length, 0);

            writer.Close();
        }
    }
}
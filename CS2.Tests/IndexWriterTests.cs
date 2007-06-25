using System.Diagnostics;
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

            Debug.WriteLine(writer.GetDirectory().GetType());

            writer.Close();
        }

        [Test]
        public void DirectoryContainsFiles()
        {
            IndexWriter writer = container.Resolve<IndexWriter>();

            Assert.Greater(writer.GetDirectory().List().Length, 0);
            Debug.WriteLine(writer.GetDirectory().GetType());

            writer.Close();
        }

        [Test]
        public void AreSame()
        {
            IndexWriter writer1 = container.Resolve<IndexWriter>();

            writer1.Close();

            IndexWriter writer2 = container.Resolve<IndexWriter>();

            writer2.Close();

            Assert.AreSame(writer1, writer2);
        }
    }
}
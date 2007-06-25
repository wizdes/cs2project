using Lucene.Net.Store;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class FileSystemDirectoryTests : BaseTest
    {
        [Test]
        public void CanResolve()
        {
            Directory dir = container.Resolve<FSDirectory>();
            
            Assert.IsNotNull(dir);
        }

        [Test]
        public void SameInstance()
        {
            Directory dir1 = container.Resolve<FSDirectory>();
            Directory dir2 = container.Resolve<FSDirectory>();

            Assert.AreSame(dir1, dir2);
        }

        [Test]
        public void SameInstanceIfClosed()
        {
            Directory dir1 = container.Resolve<FSDirectory>();
            dir1.Close();

            Directory dir2 = container.Resolve<FSDirectory>();

            Assert.AreSame(dir1, dir2);
        }
    }
}
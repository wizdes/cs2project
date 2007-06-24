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
    }
}
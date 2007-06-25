using System.Diagnostics;
using Lucene.Net.Store;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class RamDirectoryTests : BaseTest
    {
        [Test]
        public void CanResolve()
        {
            Directory dir = container.Resolve<RAMDirectory>();

            Debug.WriteLine(dir.GetType());

            Assert.IsNotNull(dir);
        }

        [Test]
        public void SameInstance()
        {
            Directory dir1 = container.Resolve<RAMDirectory>();
            Directory dir2 = container.Resolve<RAMDirectory>();
           
            Assert.AreSame(dir1, dir2);
        }
    }
}
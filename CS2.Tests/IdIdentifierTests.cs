using System;
using System.IO;
using CS2.Services;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class IdIdentifierTests
    {
        private static readonly string filePath = @"..\..\DummyClassForParseTesting.cs";

        [Test]
        public void ShouldBeEqualIDs()
        {
            string id1 = IdIdentifierUtilities.GetIdentifierFromFile(new FileInfo(filePath));
            string id2 = IdIdentifierUtilities.GetIdentifierFromFile(new FileInfo(filePath));

            Assert.AreEqual(id1, id2);
        }

        [Test]
        public void ShouldBeDifferentIDs()
        {
            string id1 = IdIdentifierUtilities.GetIdentifierFromFile(new FileInfo(filePath));

            File.SetLastWriteTime(filePath, DateTime.Now);

            string id2 = IdIdentifierUtilities.GetIdentifierFromFile(new FileInfo(filePath));

            Assert.AreNotEqual(id1, id2);
        }
    }
}
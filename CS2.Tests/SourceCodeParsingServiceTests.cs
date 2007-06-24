using System.IO;
using CS2.Services;
using DDW;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class SourceCodeParsingServiceTests : BaseTest
    {
        [Test]
        public void CanResolve()
        {
            Assert.IsNotNull(container.Resolve<ISourceCodeParsingService>());
        }

        [Test]
        public void CanParseFile()
        {
            ISourceCodeParsingService parsingService = container.Resolve<ISourceCodeParsingService>();

            CompilationUnitNode comp = parsingService.Parse(new FileInfo("..\\..\\BaseTest.Cs"));

            Assert.AreEqual(comp.Namespaces.Count, 1);
            Assert.AreEqual(comp.Namespaces[0].Classes.Count, 1);
        }
    }
}
using System.Diagnostics;
using System.IO;
using CS2.Services;
using Lucene.Net.Documents;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class ParsingServiceTests : BaseTest
    {
        [Test]
        public void CanResolve()
        {
            Assert.IsNotNull(container.Resolve<IParsingService>());
        }

        [Test]
        public void CanParseFile()
        {
            Document document = new Document();
            IParsingService parsingService = container.Resolve<IParsingService>();

            parsingService.Parse(new FileInfo("..\\..\\DummyClassForParseTesting.cs"), document);

            foreach(Field field in document.Fields())
            {
                Debug.WriteLine(string.Format("{0} {1}", field.Name(), field.StringValue()));
            }

            Assert.Greater(document.GetFieldsCount(), 0);
        }
    }
}
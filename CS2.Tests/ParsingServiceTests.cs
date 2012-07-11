using System.Diagnostics;
using System.IO;
using CS2.CSharp.Parsing;
using CS2.Core.Parsing;
using Lucene.Net.Documents;
using NUnit.Framework;

namespace CS2.Tests
{
    [TestFixture]
    public class ParsingServiceTests : BaseTest
    {
        [Test]
        public void CanParseFile()
        {
            Document document;
            IParsingService parsingService = new CSharpParsingService();

            Assert.IsTrue(parsingService.TryParse(new FileInfo("..\\..\\DummyClassForParseTesting.cs"), out document));

            foreach (Field field in document.Fields())
            {
                Debug.WriteLine(string.Format("{0} {1}", field.Name(), field.StringValue()));
            }

            Assert.Greater(document.GetFieldsCount(), 0);
        }
    }
}
using System.IO;
using Castle.Core.Logging;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace CS2.Services
{
    public class CSharpIndexingService : AbstractIndexingService, ILoggedService
    {
        private ILogger logger = NullLogger.Instance;

        public CSharpIndexingService(IndexWriter indexWriter, IParsingService parsingService, string fileMatchPattern)
            : base(indexWriter, parsingService, fileMatchPattern)
        {}

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        public override void Index(FileInfo file)
        {
            Document document = new Document();
            parsingService.Parse(file, document);

            document.Add(FieldFactory.CreatePathField(file.FullName));
            document.Add(FieldFactory.CreateFileNameField(file.Name));
            document.Add(FieldFactory.CreateSourceField(new StreamReader(file.FullName, true)));
            document.Add(
                FieldFactory.CreateModifiedField(DateTools.DateToString(file.LastWriteTime, DateTools.Resolution.HOUR)));

            Logger.InfoFormat("Indexing {0}", file.FullName);
            indexWriter.AddDocument(document);

            base.Index(file);
        }
    }
}
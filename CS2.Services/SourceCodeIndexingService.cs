using System.IO;
using DDW;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace CS2.Services
{
    public class SourceCodeIndexingService : ISourceCodeIndexingService
    {
        private readonly IndexWriter indexWriter;
        private readonly ISourceCodeParsingService parsingService;

        public IndexWriter IndexWriter
        {
            get { return indexWriter; }
        }

        public ISourceCodeParsingService ParsingService
        {
            get { return parsingService; }
        }

        public SourceCodeIndexingService(IndexWriter indexWriter, ISourceCodeParsingService parsingService)
        {
            this.indexWriter = indexWriter;
            this.parsingService = parsingService;
        }

        #region ISourceCodeIndexingService Members

        public void Index(DirectoryInfo documentsPath)
        {
            foreach(FileInfo file in documentsPath.GetFiles("*.cs", SearchOption.AllDirectories))
                IndexFile(file);

            indexWriter.Optimize();
            indexWriter.Close();
        }

        #endregion

        private void IndexFile(FileInfo file)
        {
            Document document = new Document();
            CompilationUnitNode compilationUnitNode = parsingService.Parse(file);
            SourceCodeVisitor visitor = new SourceCodeVisitor(document);

            document.Add(FieldFactory.CreatePathField(file.FullName));
            document.Add(FieldFactory.CreateFileNameField(file.Name));
            document.Add(FieldFactory.CreateSourceField(new StreamReader(file.FullName, true)));
            document.Add(FieldFactory.CreateModifiedField(DateTools.DateToString(file.LastWriteTime, DateTools.Resolution.HOUR)));
            compilationUnitNode.AcceptVisitor(visitor, null);

            indexWriter.AddDocument(document);
        }
    }
}
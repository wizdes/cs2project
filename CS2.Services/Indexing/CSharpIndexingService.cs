using System.IO;
using CS2.Model;
using CS2.Services.Parsing;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace CS2.Services.Indexing
{
    public class CSharpIndexingService : IIndexingService
    {
        private readonly IndexWriter indexWriter;
        private readonly IProgrammingLanguage language = new CSharpLanguage();
        private readonly IParsingService parsingService;

        public CSharpIndexingService(IndexWriter indexWriter, IParsingService parsingService)
        {
            this.indexWriter = indexWriter;
            this.parsingService = parsingService;
        }

        #region IIndexingService Members

        public IProgrammingLanguage Language
        {
            get { return language; }
        }

        public IndexWriter IndexWriter
        {
            get { return indexWriter; }
        }

        public IParsingService ParsingService
        {
            get { return parsingService; }
        }

        public void Index(FileInfo file)
        {
            Index(file, true);
        }

        public void Index(DirectoryInfo documentsPath)
        {
            foreach(FileInfo file in documentsPath.GetFiles(language.FileExtension, SearchOption.AllDirectories))
                Index(file, false);

            CloseWriter();
        }

        #endregion

        private void Index(FileInfo file, bool closeWriter)
        {
            Document document = new Document();
            parsingService.Parse(file, document);

            document.Add(FieldFactory.CreatePathField(file.FullName));
            document.Add(FieldFactory.CreateFileNameField(file.Name));
            document.Add(FieldFactory.CreateSourceField(new StreamReader(file.FullName, true)));
            document.Add(
                FieldFactory.CreateModifiedField(DateTools.DateToString(file.LastWriteTime, FieldFactory.ModifiedResolution)));

            indexWriter.AddDocument(document);

            if(closeWriter) CloseWriter();
        }

        private void CloseWriter()
        {
            indexWriter.Optimize();
            indexWriter.Close();
        }
    }
}
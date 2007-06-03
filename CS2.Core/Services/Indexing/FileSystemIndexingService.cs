using System;
using System.IO;
using System.Text;
using CS2.Core.Models;
using CS2.Core.Services.Parsing;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace CS2.Core.Services.Indexing
{
    class FileSystemIndexingService : IIndexingService
    {
        private IndexWriter indexWriter;
        private ISourceCodeParsingService parsingService;

        public FileSystemIndexingService(IndexWriter indexWriter, ISourceCodeParsingService parsingService)
        {
            this.indexWriter = indexWriter;
            this.parsingService = parsingService;
        }

        public void Index(FileInfo localPath)
        {
            if (!localPath.Exists)
                throw new DirectoryNotFoundException(string.Format("{0} doesn't exist.", localPath));

            try
            {
                IndexDocumentsInFolder(localPath);

                indexWriter.Optimize();
                indexWriter.Close();
            }
            catch (IOException)
            {
            }
        }

        public void IndexDocumentsInFolder(FileInfo fileOrDirectory)
        {
            if (Directory.Exists(fileOrDirectory.FullName))
            {
                String[] files = Directory.GetFileSystemEntries(fileOrDirectory.FullName);

                if (files != null)
                    foreach (string file in files)
                        IndexDocumentsInFolder(new FileInfo(file));
            }
            else
                indexWriter.AddDocument(IndexDocument(fileOrDirectory));
        }

        public Document IndexDocument(FileInfo f)
        {
            IParsedSourceFile parsedFile = parsingService.Parse(f);

            // Make a new, empty document
            Document doc = new Document();
            // Add the path of the file as a field named "path".
            // Use a field that is indexed (i.e., searchable), but don't
            // tokenize the field into words.
            doc.Add(new Field("path", f.FullName, Field.Store.YES, Field.Index.UN_TOKENIZED));
            // Add the last modified date of the file to a field named
            // "modified". Use a field that is indexed (i.e., searchable),
            // but don't tokenize the field into words.
            doc.Add(new Field("modified",

            DateTools.TimeToString(f.LastWriteTime.Ticks, DateTools.Resolution.MINUTE), Field.Store.YES, Field.Index.UN_TOKENIZED));
            // Add the contents of the file to a field named "contents".
            // Specify a Reader, so that the text of the file is tokenized
            // and indexed, but not stored. Note that FileReader expects
            // the file to be in the system's default encoding. If that's
            // not the case, searching for special characters will fail.
            doc.Add(new Field("contents", new StreamReader(f.FullName, Encoding.Default)));
            // Return the document
            return doc;
        }
    }
}

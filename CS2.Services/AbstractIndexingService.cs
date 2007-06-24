using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Lucene.Net.Index;

namespace CS2.Services
{
    public abstract class AbstractIndexingService
    {
        protected readonly IndexWriter indexWriter;
        protected readonly IParsingService parsingService;
        private readonly string fileMatchPattern;

        protected AbstractIndexingService(IndexWriter indexWriter, IParsingService parsingService, string fileMatchPattern)
        {
            this.indexWriter = indexWriter;
            this.parsingService = parsingService;
            this.fileMatchPattern = fileMatchPattern;
        }

        public IndexWriter IndexWriter
        {
            get { return indexWriter; }
        }

        public IParsingService ParsingService
        {
            get { return parsingService; }
        }

        public string FileMatchPattern
        {
            get { return fileMatchPattern; }
        }

        public void Index(DirectoryInfo documentsPath)
        {
            foreach(FileInfo file in documentsPath.GetFiles(fileMatchPattern, SearchOption.AllDirectories))
                Index(file);

            indexWriter.Optimize();
            indexWriter.Close();
        }

        public virtual void Index(FileInfo file)
        {
            //if(GetCallingMethod().DeclaringType != typeof(AbstractIndexingService))
            //{
            //    indexWriter.Optimize();
            //    indexWriter.Close();
            //}
        }

        private static MethodBase GetCallingMethod()
        {
            return new StackFrame(2, false).GetMethod();
        }
    }
}
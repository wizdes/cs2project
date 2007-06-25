using System.IO;
using Castle.Core.Logging;
using CS2.Model;
using CS2.Services.Indexing;
using CS2.Services.Parsing;
using Lucene.Net.Index;

namespace CS2.Services.Logging
{
    public class LoggingIndexingDecorator : IIndexingService, ILoggingService
    {
        private readonly IIndexingService inner;
        private ILogger logger = NullLogger.Instance;

        public LoggingIndexingDecorator(IIndexingService inner)
        {
            this.inner = inner;
        }

        #region IIndexingService Members

        public void Index(FileInfo file)
        {
            logger.InfoFormat("Start indexing file {0}", file.FullName);
            inner.Index(file);
            logger.InfoFormat("Start indexing file {0}", file.FullName);
        }

        public void Index(DirectoryInfo directory)
        {
            logger.InfoFormat("Start indexing directory {0}", directory.FullName);
            inner.Index(directory);
            logger.InfoFormat("Done indexing directory {0}", directory.FullName);
        }

        public IProgrammingLanguage Language
        {
            get { return inner.Language; }
        }

        public IndexWriter IndexWriter
        {
            get { return inner.IndexWriter; }
        }

        public IParsingService ParsingService
        {
            get { return inner.ParsingService; }
        }

        #endregion

        #region ILoggingService Members

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        #endregion
    }
}
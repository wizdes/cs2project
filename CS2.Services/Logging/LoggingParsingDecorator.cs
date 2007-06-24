using System.IO;
using Castle.Core.Logging;
using CS2.Services.Logging;
using CS2.Services.Parsing;
using Lucene.Net.Documents;

namespace CS2.Services.Logging
{
    public class LoggingParsingDecorator : IParsingService, ILoggingService
    {
        private readonly IParsingService inner;
        private ILogger logger = NullLogger.Instance;

        public LoggingParsingDecorator(IParsingService inner)
        {
            this.inner = inner;
        }

        #region ILoggingService Members

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        #endregion

        #region IParsingService Members

        public void Parse(FileInfo file, Document document)
        {
            Logger.DebugFormat("Start parsing file {0}", file.FullName);
            inner.Parse(file, document);
            Logger.DebugFormat("Done parsing file {0}", file.FullName);
        }

        #endregion
    }
}
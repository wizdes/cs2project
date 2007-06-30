using System.IO;
using Castle.Core.Logging;
using CS2.Services.Logging;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;

namespace CS2.Services.Parsing
{
    public class LoggedParsingService : IParsingService, ILoggingService
    {
        private readonly IParsingService inner;
        private ILogger logger = NullLogger.Instance;

        public LoggedParsingService(IParsingService inner)
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

        public string[] Exclusions
        {
            get { return inner.Exclusions; }
            set { inner.Exclusions = value; }
        }

        public Analyzer Analyzer
        {
            get { return inner.Analyzer; }
        }

        public bool TryParse(FileInfo file, out Document document)
        {
            Logger.DebugFormat("Start parsing file {0}", file.FullName);

            bool couldParse = inner.TryParse(file, out document);

            Logger.DebugFormat(couldParse ? "Done parsing file {0}" : "Error parsing file {0}", file.FullName);

            return couldParse;
        }

        #endregion
    }
}
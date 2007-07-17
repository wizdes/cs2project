using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Castle.Core.Logging;
using CS2.Core.Analysis;
using CS2.Core.Logging;
using Lucene.Net.Documents;

namespace CS2.Core.Parsing
{
    public class LoggedParsingService : IParsingService, ILoggingService
    {
        private readonly IParsingService inner;
        private readonly Stopwatch watch = new Stopwatch();
        private ILogger logger = NullLogger.Instance;

        public LoggedParsingService(IParsingService inner)
        {
            this.inner = inner;
            watch.Start();
            watch.Reset();
        }

        #region ILoggingService Members

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        #endregion

        #region IParsingService Members

        public ICollection<string> FileExtensions
        {
            get { return inner.FileExtensions; }
        }

        public string LanguageName
        {
            get { return inner.LanguageName; }
        }

        public AbstractAnalyzer Analyzer
        {
            get { return inner.Analyzer; }
        }

        public bool TryParse(FileInfo file, out Document document)
        {
            watch.Start();

            bool couldParse = inner.TryParse(file, out document);
            long elapsed = watch.ElapsedMilliseconds;

            watch.Reset();

            if(couldParse)
                Trace.TraceInformation("Done parsing file {0} in {1} milliseconds", file.FullName, elapsed);
            else
                Trace.TraceError("Error parsing file {0} in {1} milliseconds", file.FullName, elapsed);

            return couldParse;
        }

        #endregion
    }
}
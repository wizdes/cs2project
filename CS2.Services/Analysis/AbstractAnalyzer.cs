using System.Collections.Generic;
using Lucene.Net.Analysis;

namespace CS2.Services.Analysis
{
    public abstract class AbstractAnalyzer : Analyzer
    {
        protected abstract ICollection<string> StopWords { get; }
        public abstract string SearchPattern { get; }
        public abstract string LanguageName { get; }
    }
}
using System.Collections.Generic;
using Lucene.Net.Analysis;

namespace CS2.Core.Analysis
{
    public abstract class AbstractAnalyzer : Analyzer
    {
        protected abstract ICollection<string> StopWords { get; }
    }
}
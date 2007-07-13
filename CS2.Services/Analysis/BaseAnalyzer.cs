using CS2.Model;
using Lucene.Net.Analysis;

namespace CS2.Services.Analysis
{
    public abstract class BaseAnalyzer : Analyzer
    {
        private readonly IProgrammingLanguage language;

        public IProgrammingLanguage Language
        {
            get { return language; }
        }

        protected BaseAnalyzer(IProgrammingLanguage language)
        {
            this.language = language;
        }
    }
}
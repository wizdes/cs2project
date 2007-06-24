using CS2.Model;
using Lucene.Net.Analysis;

namespace CS2.Services.Analysis
{
    public abstract class GenericAnalyzer<T> : Analyzer where T : IProgrammingLanguage, new()
    {
        private readonly T language = new T();

        protected T Language
        {
            get { return language; }
        }
    }
}
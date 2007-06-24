using System.IO;
using CS2.Model;
using CS2.Services.Parsing;
using Lucene.Net.Index;

namespace CS2.Services.Indexing
{
    public interface IIndexingService
    {
        IProgrammingLanguage Language { get; }

        IndexWriter IndexWriter { get; }

        IParsingService ParsingService { get; }

        void Index(FileInfo file);
        void Index(DirectoryInfo directory);
    }
}
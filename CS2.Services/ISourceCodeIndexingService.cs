using System.IO;
using Lucene.Net.Index;

namespace CS2.Services
{
    public interface ISourceCodeIndexingService
    {
        IndexWriter IndexWriter { get; }

        ISourceCodeParsingService ParsingService { get; }

        void Index(DirectoryInfo documentsPath);
    }
}
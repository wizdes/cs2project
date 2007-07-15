using System.Collections.Generic;
using System.IO;
using CS2.Core.Analysis;
using Lucene.Net.Documents;

namespace CS2.Core.Parsing
{
    public interface IParsingService
    {
        /// <summary>
        /// Gets or sets the analyzer used to index the documents supported by this parser.
        /// </summary>
        /// <value>The analyzer.</value>
        AbstractAnalyzer Analyzer { get; }

        string LanguageName { get; }

        ICollection<string> FileExtensions { get; }

        /// <summary>
        /// Tries to parse the specified file into the supplied document.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="document">The document.</param>
        /// <returns>True if the parsing is successful, false otherwise.</returns>
        bool TryParse(FileInfo file, out Document document);
    }
}
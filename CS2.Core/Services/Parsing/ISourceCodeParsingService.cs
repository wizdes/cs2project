using System.IO;
using CS2.Core.Models;

namespace CS2.Core.Services.Parsing
{
    interface ISourceCodeParsingService
    {
        IParsedSourceFile Parse(FileInfo sourceFile);
    }
}

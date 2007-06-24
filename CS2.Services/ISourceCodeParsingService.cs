using System.IO;
using DDW;

namespace CS2.Services
{
    public interface ISourceCodeParsingService
    {
        CompilationUnitNode Parse(FileInfo file);
    }
}
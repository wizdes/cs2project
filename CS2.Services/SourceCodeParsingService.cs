using System.IO;
using DDW;
using DDW.Collections;

namespace CS2.Services
{
    public class SourceCodeParsingService : ISourceCodeParsingService
    {
        #region ISourceCodeParsingService Members

        public CompilationUnitNode Parse(FileInfo file)
        {
            FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
            CompilationUnitNode compilationUnitNode;
            Parser parser;
            Lexer lexer;
            TokenCollection tokens;

            using(StreamReader reader = new StreamReader(fileStream, true))
            {
                lexer = new Lexer(reader);
                tokens = lexer.Lex();
            }

            parser = new Parser(file.FullName);
            compilationUnitNode = parser.Parse(tokens, lexer.StringLiterals);

            return compilationUnitNode;
        }

        #endregion
    }
}
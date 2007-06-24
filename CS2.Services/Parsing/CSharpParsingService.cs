using System.IO;
using DDW;
using DDW.Collections;
using Lucene.Net.Documents;

namespace CS2.Services.Parsing
{
    public class CSharpParsingService : IParsingService
    {
        private readonly IParsingVisitor parsingVisitor;

        public CSharpParsingService(IParsingVisitor parsingVisitor)
        {
            this.parsingVisitor = parsingVisitor;
        }

        #region IParsingService Members

        public void Parse(FileInfo file, Document document)
        {
            FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
            Lexer lexer;
            TokenCollection tokens;

            using(StreamReader reader = new StreamReader(fileStream, true))
            {
                lexer = new Lexer(reader);
                tokens = lexer.Lex();
            }

            Parser parser = new Parser(file.FullName);

            CompilationUnitNode compilationUnitNode = parser.Parse(tokens, lexer.StringLiterals);

            compilationUnitNode.AcceptVisitor((AbstractVisitor) parsingVisitor, document);
        }

        #endregion
    }
}
using System.IO;
using DDW;
using DDW.Collections;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;

namespace CS2.Services.Parsing
{
    public class CSharpParsingService : IParsingService
    {
        private readonly IParsingVisitor parsingVisitor;
        private readonly Analyzer analyzer;

        public CSharpParsingService(IParsingVisitor parsingVisitor, Analyzer analyzer)
        {
            this.parsingVisitor = parsingVisitor;
            this.analyzer = analyzer;
        }

        #region IParsingService Members

        /// <summary>
        /// Tries to parse the specified file into the supplied document.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="document">The document.</param>
        /// <returns>True is the parsing is successful, false otherwise.</returns>
        public bool TryParse(FileInfo file, out Document document)
        {
            document = new Document();

            try
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

                // Too few fields found, this is probably not a C# file
                return document.GetFieldsCount() > 1 ? true : false;
            }
            catch
            {
                document = null;
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the analyzer used to index the documents supported by this parser.
        /// </summary>
        /// <value>The analyzer.</value>
        public Analyzer Analyzer
        {
            get { return analyzer; }
        }

        #endregion
    }
}
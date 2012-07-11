using System.Collections.Generic;
using System.IO;
using System.Threading;
using CS2.Core.Analysis;
using CS2.Core.Parsing;
using CS2.CSharp.Analysis;
using DDW;
using DDW.Collections;
using Lucene.Net.Documents;

namespace CS2.CSharp.Parsing
{
    public class CSharpParsingService : IParsingService
    {
        private readonly AbstractAnalyzer analyzer;

        private readonly CSharpParsingVisitor parsingVisitor;

        public CSharpParsingService()
        {
            parsingVisitor = new CSharpParsingVisitor();
            analyzer = new CSharpAnalyzer();
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
                var parser = new Thread(Parse);
                parser.Start(new object[] { file, document });

                if(parser.Join(1000))
                    // Too few fields found, this is probably not a C# file
                    return document.GetFieldsCount() > 1;
                
                parser.Abort();
                return false;
            }
            catch
            {
                document = null;
                return false;
            }
        }

        public ICollection<string> SupportedFileExtensions
        {
            get { return new[] { ".cs" }; }
        }

        public string LanguageName
        {
            get { return "c#"; }
        }

        /// <summary>
        /// Gets or sets the analyzer used to index the documents supported by this parser.
        /// </summary>
        /// <value>The analyzer.</value>
        public AbstractAnalyzer Analyzer
        {
            get { return analyzer; }
        }

        #endregion

        private void Parse(object data)
        {
            Lexer lexer;

            object[] bag = data as object[];

            if(bag == null)
                return;

            FileInfo file = bag[0] as FileInfo;
            Document document = bag[1] as Document;

            if(file == null || document == null)
                return;

            TokenCollection tokens = null;

            try
            {
                using(FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                using(StreamReader reader = new StreamReader(fileStream, true))
                {
                    lexer = new Lexer(reader);
                    tokens = lexer.Lex();
                }

                Parser parser = new Parser(file.FullName);

                CompilationUnitNode compilationUnitNode = parser.Parse(tokens, lexer.StringLiterals);

                compilationUnitNode.AcceptVisitor(parsingVisitor, document);
            }
            catch
            {
                if(tokens != null)
                    tokens.Clear();
            }
        }
    }
}
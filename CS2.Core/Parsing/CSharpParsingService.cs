using System;
using System.Collections.Generic;
using System.IO;
using CS2.Core.Analysis;
using DDW;
using DDW.Collections;
using Lucene.Net.Documents;

namespace CS2.Core.Parsing
{
    public class CSharpParsingService : IParsingService
    {
        #region Delegates

        public delegate CompilationUnitNode ParseDelegate(Parser parser, TokenCollection tokens, List<string> literals);

        #endregion

        private readonly AbstractAnalyzer analyzer;

        private readonly ParseDelegate parseDelegate =
            delegate(Parser parser, TokenCollection toks, List<string> literals) { return parser.Parse(toks, literals); };

        private readonly IParsingVisitor parsingVisitor;

        public CSharpParsingService(IParsingVisitor parsingVisitor, AbstractAnalyzer analyzer)
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
            Lexer lexer;
            document = new Document();
            TokenCollection tokens = null;

            try
            {
                FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);

                using(StreamReader reader = new StreamReader(fileStream, true))
                {
                    lexer = new Lexer(reader);
                    tokens = lexer.Lex();
                }

                Parser parser = new Parser(file.FullName);
                IAsyncResult result = parseDelegate.BeginInvoke(parser, tokens, lexer.StringLiterals, null, null);

                // Call asynchronously and wait for a while for the parsing to complete
                if(result.AsyncWaitHandle.WaitOne(1500, true))
                {
                    CompilationUnitNode compilationUnitNode = parseDelegate.EndInvoke(result);
                    compilationUnitNode.AcceptVisitor((AbstractVisitor) parsingVisitor, document);

                    // Too few fields found, this is probably not a C# file
                    return document.GetFieldsCount() > 1 ? true : false;
                }
                    // The parsing didn't complete in time or threw an exception
                else
                    throw new InvalidOperationException("Didn't complete in time.");

//                CompilationUnitNode compilationUnitNode = parser.Parse(tokens, lexer.StringLiterals);
//
//                compilationUnitNode.AcceptVisitor((AbstractVisitor)parsingVisitor, document);
//
//                // Too few fields found, this is probably not a C# file
//                return document.GetFieldsCount() > 1 ? true : false;
            }
            catch
            {
                document = null;
                return false;
            }
            finally
            {
                if(tokens != null)
                    tokens.Clear();
            }
        }

        public ICollection<string> FileExtensions
        {
            get { return new string[] { ".cs" }; }
        }

        public string LanguageName
        {
            get { return "C#"; }
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
    }
}
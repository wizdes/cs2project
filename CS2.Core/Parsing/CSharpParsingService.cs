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

        public delegate CompilationUnitNode ParseDelegate(TokenCollection tokens, List<string> literals);

        #endregion

        private readonly AbstractAnalyzer analyzer;
        private readonly IParsingVisitor parsingVisitor;
        private string[] exclusions;

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
            document = new Document();
            Lexer lexer;
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

                ParseDelegate del = delegate(TokenCollection toks, List<string> literals) { return parser.Parse(toks, literals); };

                IAsyncResult result = del.BeginInvoke(tokens, lexer.StringLiterals, null, null);

                // Call asynchronously and wait for 500 ms for the parsing to complete
                if(result.AsyncWaitHandle.WaitOne(200, true))
                {
                    CompilationUnitNode compilationUnitNode = del.EndInvoke(result);
                    compilationUnitNode.AcceptVisitor((AbstractVisitor) parsingVisitor, document);

                    // Too few fields found, this is probably not a C# file
                    return document.GetFieldsCount() > 1 ? true : false;
                }
                // The parsing didn't complete in 500 ms or threw an exception
                else
                {
                    document = null;
                    return false;
                }

//                CompilationUnitNode compilationUnitNode = parser.Parse(tokens, lexer.StringLiterals);

//                compilationUnitNode.AcceptVisitor((AbstractVisitor) parsingVisitor, document);
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

        public string[] Exclusions
        {
            get { return exclusions; }
            set { exclusions = value; }
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
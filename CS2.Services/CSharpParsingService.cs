using System;
using System.IO;
using Castle.Core.Logging;
using DDW;
using DDW.Collections;
using Lucene.Net.Documents;

namespace CS2.Services
{
    public class CSharpParsingService : IParsingService, ILoggedService
    {
        private readonly IParsingVisitor visitor;
        private ILogger logger = NullLogger.Instance;

        public CSharpParsingService(IParsingVisitor visitor)
        {
            this.visitor = visitor;
        }

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
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

            try
            {
                Logger.InfoFormat("Parsing {0}", file.FullName);
                CompilationUnitNode compilationUnitNode = parser.Parse(tokens, lexer.StringLiterals);

                compilationUnitNode.AcceptVisitor((AbstractVisitor) visitor, document);
            }
            catch(Exception ex)
            {
                Logger.Error(file.FullName, ex);
            }
        }

        #endregion
    }
}
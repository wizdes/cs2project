using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using CS2.Core.Analysis;
using Lucene.Net.Analysis;

namespace CS2.Core.Analysis
{
    public class CSharpAnalyzer : AbstractAnalyzer
    {
        protected override ICollection<string> StopWords
        {
            get
            {
                return
                    new Collection<string>(
                        new string[]
                            {
                                "byte", "bool", "char", "double", "decimal", "float", "int", "long", "object", "sbyte", "string",
                                "short", "ushort", "ulong", "uint", "abstract", "const", "extern", "alias", "explicit", "implicit",
                                "internal", "new", "out", "override", "private", "public", "protected", "ref", "readonly", "static",
                                "sealed", "volatile", "virtual", "class", "delegate", "enum", "interface", "struct", "as", "base",
                                "break", "catch", "continue", "case", "do", "default", "else", "for", "foreach", "finally", "fixed",
                                "goto", "if", "in", "is", "lock", "return", "stackalloc", "switch", "sizeof", "throw", "try",
                                "typeof", "this", "void", "while", "checked", "event", "namespace", "operator", "params", "unsafe",
                                "unchecked", "using", "where", "partial", "yield", "true", "false", "null"
                                //, "assembly", "property", "method", "field", "param", "type"                 
                            });
            }
        }

        public override string SearchPattern
        {
            get { return "*.cs"; }
        }

        public override string LanguageName
        {
            get { return "C#"; }
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            if(fieldName.Equals(FieldFactory.CommentFieldName))
                return new PorterStemFilter(new StopFilter(new LowerCaseTokenizer(reader), StopAnalyzer.ENGLISH_STOP_WORDS));
            else
                return new StopFilter(new LowerCaseTokenizer(reader), new List<string>(StopWords).ToArray());
        }
    }
}
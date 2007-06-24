using System.IO;
using Lucene.Net.Analysis;

namespace CS2.Services
{
    public class CSharpAnalyzer : Analyzer
    {
        private static readonly string[] cSharpStopwords = new string[]
            {
                "byte", "bool", "char", "double", "decimal", "float", "int", "long", "object", "sbyte", "string",
                "short", "ushort", "ulong", "uint", "abstract", "const", "extern", "alias", "explicit", "implicit",
                "internal", "new", "out", "override", "private", "public", "protected", "ref", "readonly", "static",
                "sealed", "volatile", "virtual", "class", "delegate", "enum", "interface", "struct", "as", "base",
                "break", "catch", "continue", "case", "do", "default", "else", "for", "foreach", "finally", "fixed",
                "goto", "if", "in", "is", "lock", "return", "stackalloc", "switch", "sizeof", "throw", "try", "typeof",
                "this", "void", "while", "checked", "event", "namespace", "operator", "params", "unsafe", "unchecked",
                "using", "where", "partial", "yield", "true", "false", "null"
                //"assembly", "property", "method", "field", "param", "type",                  
            };

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            if(fieldName.Equals("comment"))
                return
                    new PorterStemFilter(new StopFilter(new LowerCaseTokenizer(reader), StopAnalyzer.ENGLISH_STOP_WORDS));
            else return new StopFilter(new LowerCaseTokenizer(reader), cSharpStopwords);
        }
    }
}
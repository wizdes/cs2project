namespace CS2.Model
{
    public class CSharpLanguage : IProgrammingLanguage
    {
        #region IProgrammingLanguage Members

        public string[] StopWords
        {
            get
            {
                return new string[]
                    {
                        "byte", "bool", "char", "double", "decimal", "float", "int", "long", "object", "sbyte", "string"
                        , "short", "ushort", "ulong", "uint", "abstract", "const", "extern", "alias", "explicit",
                        "implicit", "internal", "new", "out", "override", "private", "public", "protected", "ref",
                        "readonly", "static", "sealed", "volatile", "virtual", "class", "delegate", "enum", "interface",
                        "struct", "as", "base", "break", "catch", "continue", "case", "do", "default", "else", "for",
                        "foreach", "finally", "fixed", "goto", "if", "in", "is", "lock", "return", "stackalloc",
                        "switch", "sizeof", "throw", "try", "typeof", "this", "void", "while", "checked", "event",
                        "namespace", "operator", "params", "unsafe", "unchecked", "using", "where", "partial", "yield",
                        "true", "false", "null"
                        //, "assembly", "property", "method", "field", "param", "type"                 
                    };
            }
        }

        public string FileExtension
        {
            get { return "*.cs"; }
        }

        #endregion
    }
}
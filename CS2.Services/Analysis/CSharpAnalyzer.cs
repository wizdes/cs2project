using System.Collections.Generic;
using System.IO;
using CS2.Model;
using Lucene.Net.Analysis;

namespace CS2.Services.Analysis
{
    public class CSharpAnalyzer : BaseAnalyzer
    {
        public CSharpAnalyzer() : base(new CSharpLanguage())
        {
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            if(fieldName.Equals(FieldFactory.CommentFieldName))
                return
                    new PorterStemFilter(new StopFilter(new LowerCaseTokenizer(reader), StopAnalyzer.ENGLISH_STOP_WORDS));
            else return new StopFilter(new LowerCaseTokenizer(reader), new List<string>(Language.StopWords).ToArray());
        }
    }
}
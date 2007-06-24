using System.IO;
using CS2.Model;
using Lucene.Net.Analysis;

namespace CS2.Services.Analysis
{
    public class CSharpAnalyzer : GenericAnalyzer<CSharpLanguage>
    {
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            if(fieldName.Equals("comment"))
                return
                    new PorterStemFilter(new StopFilter(new LowerCaseTokenizer(reader), StopAnalyzer.ENGLISH_STOP_WORDS));
            else return new StopFilter(new LowerCaseTokenizer(reader), Language.StopWords);
        }
    }
}
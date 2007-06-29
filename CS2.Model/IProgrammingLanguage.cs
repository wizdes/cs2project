using System.Collections.Generic;

namespace CS2.Model
{
    public interface IProgrammingLanguage
    {
        ICollection<string> StopWords { get; }
        string SearchPattern { get; }
    }
}
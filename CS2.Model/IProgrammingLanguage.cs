namespace CS2.Model
{
    public interface IProgrammingLanguage
    {
        string[] StopWords { get; }
        string SearchPattern { get; }
    }
}
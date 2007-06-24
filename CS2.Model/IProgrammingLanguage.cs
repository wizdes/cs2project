namespace CS2.Model
{
    public interface IProgrammingLanguage
    {
        string[] StopWords { get; }

        string FileExtension { get; }
    }
}
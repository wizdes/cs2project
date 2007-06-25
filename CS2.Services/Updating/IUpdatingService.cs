using Lucene.Net.Index;

namespace CS2.Services.Updating
{
    public interface IUpdatingService
    {
        IndexReader IndexReader { get; }

        bool NeedsUpdating { get; }

        void Update();
    }
}
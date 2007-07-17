using System.Collections.Generic;

namespace CS2.Core
{
    public interface ISynchronizedStringSet : IEnumerable<string>
    {
        int Count { get; }

        bool Add(string item);
        void Clear();
        ISynchronizedStringSet CloneAndClear();
        bool Remove(string item);
    }
}
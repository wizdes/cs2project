using System.Collections.Generic;

namespace CS2.Core
{
    public interface ISynchronizedCollection : IEnumerable<string>
    {
        int Count { get; }

        bool Add(string item);
        void Clear();
        ISynchronizedCollection CloneAndClear();
        bool Remove(string item);
    }
}
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace CS2.Services
{
    public class SynchronizedSet
    {
        private readonly Set<string> inner;
        private readonly object syncLock = new object();

        public SynchronizedSet()
        {
            inner = new Set<string>();
        }

        private SynchronizedSet(Set<string> inner)
        {
            this.inner = inner;
        }

        public bool Add(string item)
        {
            lock(syncLock)
                return inner.Add(item.ToLowerInvariant());
        }

        public void Clear()
        {
            lock(syncLock)
                inner.Clear();
        }

        public int Count
        {
            get { return inner.Count; }
        }

        public SynchronizedSet CloneAndClear()
        {
            lock(syncLock)
            {
                SynchronizedSet clone = new SynchronizedSet(inner.Clone());
                inner.Clear();
                return clone;
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        public bool Remove(string item)
        {
            lock(syncLock)
                return inner.Remove(item.ToLowerInvariant());
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace CS2.Core
{
    public class SynchronizedSet : ISynchronizedCollection
    {
        private readonly Set<string> inner;
        private readonly object syncLock = new object();

        public SynchronizedSet()
        {
            inner = new Set<string>(StringComparer.InvariantCultureIgnoreCase);
        }

        private SynchronizedSet(Set<string> inner)
        {
            this.inner = inner;
        }

        #region ISynchronizedCollection Members

        public bool Add(string item)
        {
            lock(syncLock)
                return inner.Add(item);
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

        public ISynchronizedCollection CloneAndClear()
        {
            lock(syncLock)
            {
                SynchronizedSet clone = new SynchronizedSet(inner.Clone());
                inner.Clear();
                return clone;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        public bool Remove(string item)
        {
            lock(syncLock)
                return inner.Remove(item);
        }

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
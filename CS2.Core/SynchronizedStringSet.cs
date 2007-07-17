using System;
using System.Collections;
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace CS2.Core
{
    public class SynchronizedStringSet : ISynchronizedStringSet
    {
        private readonly Set<string> set;
        private readonly object syncLock = new object();

        public SynchronizedStringSet()
        {
            set = new Set<string>(StringComparer.InvariantCultureIgnoreCase);
        }

        private SynchronizedStringSet(Set<string> inner)
        {
            set = inner;
        }

        #region ISynchronizedStringSet Members

        public bool Add(string item)
        {
            lock(syncLock)
                return set.Add(item);
        }

        public void Clear()
        {
            lock(syncLock)
                set.Clear();
        }

        public int Count
        {
            get { return set.Count; }
        }

        public ISynchronizedStringSet CloneAndClear()
        {
            lock(syncLock)
            {
                ISynchronizedStringSet clone = new SynchronizedStringSet(set.Clone());
                set.Clear();
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
            return set.GetEnumerator();
        }

        public bool Remove(string item)
        {
            lock(syncLock)
                return set.Remove(item);
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
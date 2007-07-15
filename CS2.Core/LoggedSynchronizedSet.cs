using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace CS2.Core
{
    public class LoggedSynchronizedSet : ISynchronizedCollection
    {
        private readonly ISynchronizedCollection inner;

        public LoggedSynchronizedSet(ISynchronizedCollection inner)
        {
            this.inner = inner;
        }

        #region ISynchronizedCollection Members

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
            return ((IEnumerable) inner).GetEnumerator();
        }

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        public IEnumerator<string> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        public bool Add(string item)
        {
            Thread.Sleep(100);
            Trace.TraceInformation("Adding {0} to collection. Total items: {1}", item, Count + 1);
            return inner.Add(item);
        }

        public void Clear()
        {
            Trace.TraceInformation("Clearing collection");
            inner.Clear();
        }

        public ISynchronizedCollection CloneAndClear()
        {
            Trace.TraceInformation("Cloning and clearing the collection");
            return inner.CloneAndClear();
        }

        public bool Remove(string item)
        {
            Trace.TraceInformation("Removing {0} from the collection. Total items: {1}", item, Count - 1);
            return inner.Remove(item);
        }

        public int Count
        {
            get { return inner.Count; }
        }

        #endregion
    }
}
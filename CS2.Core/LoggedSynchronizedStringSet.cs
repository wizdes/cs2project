using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Castle.Core.Logging;
using CS2.Core.Logging;

namespace CS2.Core
{
    public class LoggedSynchronizedStringSet : ISynchronizedStringSet, ILoggingService
    {
        private readonly ISynchronizedStringSet inner;
        private ILogger logger = NullLogger.Instance;

        public LoggedSynchronizedStringSet(ISynchronizedStringSet inner)
        {
            this.inner = inner;
        }

        #region ILoggingService Members

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        #endregion

        #region ISynchronizedStringSet Members

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
            Thread.Sleep(0);

            try
            {
                return inner.Add(item);
            }
            finally
            {
                Logger.InfoFormat("Adding {0} to collection. Total items: {1}", item, Count);
            }
        }

        public void Clear()
        {
            Logger.InfoFormat("Clearing collection");
            inner.Clear();
        }

        public ISynchronizedStringSet CloneAndClear()
        {
            Logger.InfoFormat("Cloning and clearing the collection. Total items: {0}", Count);
            return inner.CloneAndClear();
        }

        public bool Remove(string item)
        {
            try
            {
                return inner.Remove(item);
            }
            finally
            {
                Logger.InfoFormat("Removing {0} from the collection. Total items: {1}", item, Count);
            }
        }

        public int Count
        {
            get { return inner.Count; }
        }

        #endregion
    }
}
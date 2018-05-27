using System;
using System.Collections.Generic;
using System.Text;
using TransactionRecorder.Logging;

namespace TransactionRecorder.Core
{
    public abstract class JobStorage
    {
        private static readonly object _lockObject = new object();
        private static JobStorage _current;

        public static JobStorage Current
        {
            get
            {
                lock (_lockObject)
                {
                    if (_current == null)
                    {
                        throw new InvalidOperationException("JobStorage.Current property value has not been initialized. You must set it before using Hangfire Client or Server API.");
                    }

                    return _current;
                }
            }
            set
            {
                lock (_lockObject)
                {
                    _current = value;
                }
            }
        }

//        public abstract IMonitoringApi GetMonitoringApi();

//        public abstract IStorageConnection GetConnection();

//#pragma warning disable 618
//        public virtual IEnumerable<IServerComponent> GetComponents()
//        {
//            return Enumerable.Empty<IServerComponent>();
//        }
//#pragma warning restore 618

//        public virtual IEnumerable<IStateHandler> GetStateHandlers()
//        {
//            return Enumerable.Empty<IStateHandler>();
//        }

        public virtual void WriteOptionsToLog(ILog logger)
        {
        }
    }
}

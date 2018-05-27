using System;
using System.Threading;

namespace TransactionRecorder.Core
{
    public abstract class JobActivatorScope : IDisposable
    {
        // ReSharper disable once InconsistentNaming
        private static readonly ThreadLocal<JobActivatorScope> _current
            = new ThreadLocal<JobActivatorScope>(trackAllValues: false);

        protected JobActivatorScope()
        {
            _current.Value = this;
        }

        public static JobActivatorScope Current => _current.Value;

        public object InnerScope { get; set; }

        public abstract object Resolve(Type type);

        public virtual void DisposeScope()
        {
        }

        public void Dispose()
        {
            try
            {
                DisposeScope();
            }
            finally
            {
                _current.Value = null;
            }
        }
    }
}

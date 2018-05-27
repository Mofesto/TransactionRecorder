using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionRecorder.Core
{
    public static class GlobalJobFilters
    {
        static GlobalJobFilters()
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            Filters = new JobFilterCollection();

            // Filters should be added with the `Add` method call: some 
            // of them indirectly use `GlobalJobFilters.Filters` property, 
            // and it is null, when we are using collection initializer.
            Filters.Add(new CaptureCultureAttribute());
            Filters.Add(new AutomaticRetryAttribute());
            Filters.Add(new StatisticsHistoryAttribute());
            Filters.Add(new ContinuationsSupportAttribute());
        }

        /// <summary>
        /// Gets the global filter collection.
        /// </summary>
        public static JobFilterCollection Filters { get; }
    }
}

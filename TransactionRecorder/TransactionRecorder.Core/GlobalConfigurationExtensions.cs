using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TransactionRecorder.Logging;
using TransactionRecorder.Logging.LogProviders;

namespace TransactionRecorder.Core
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration<TStorage> UseStorage<TStorage>(
             this IGlobalConfiguration configuration,
                  TStorage storage) where TStorage : JobStorage
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (storage == null) throw new ArgumentNullException(nameof(storage));

            return configuration.Use(storage, x => JobStorage.Current = x);
        }

        public static IGlobalConfiguration<TActivator> UseActivator<TActivator>(
             this IGlobalConfiguration configuration,
             TActivator activator)
            where TActivator : JobActivator
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (activator == null) throw new ArgumentNullException(nameof(activator));

            return configuration.Use(activator, x => JobActivator.Current = x);
        }

        public static IGlobalConfiguration<JobActivator> UseDefaultActivator(
             this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseActivator(new JobActivator());
        }

        public static IGlobalConfiguration<TLogProvider> UseLogProvider<TLogProvider>(
            this IGlobalConfiguration configuration,
            TLogProvider provider)
            where TLogProvider : ILogProvider
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            return configuration.Use(provider, x => LogProvider.SetCurrentLogProvider(x));
        }

        public static IGlobalConfiguration<NLogLogProvider> UseNLogLogProvider(
             this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseLogProvider(new NLogLogProvider());
        }

        public static IGlobalConfiguration<ColouredConsoleLogProvider> UseColouredConsoleLogProvider(
             this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseLogProvider(new ColouredConsoleLogProvider());
        }

        public static IGlobalConfiguration<Log4NetLogProvider> UseLog4NetLogProvider(
             this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseLogProvider(new Log4NetLogProvider());
        }

#if NETFULL
        public static IGlobalConfiguration<ElmahLogProvider> UseElmahLogProvider(
            [NotNull] this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseLogProvider(new ElmahLogProvider());
        }

        public static IGlobalConfiguration<ElmahLogProvider> UseElmahLogProvider(
            [NotNull] this IGlobalConfiguration configuration,
            LogLevel minLevel)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseLogProvider(new ElmahLogProvider(minLevel));
        }

        public static IGlobalConfiguration<EntLibLogProvider> UseEntLibLogProvider(
            [NotNull] this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseLogProvider(new EntLibLogProvider());
        }

        public static IGlobalConfiguration<SerilogLogProvider> UseSerilogLogProvider(
            [NotNull] this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseLogProvider(new SerilogLogProvider());
        }

        public static IGlobalConfiguration<LoupeLogProvider> UseLoupeLogProvider(
            [NotNull] this IGlobalConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration.UseLogProvider(new LoupeLogProvider());
        }
#endif

        public static IGlobalConfiguration<TFilter> UseFilter<TFilter>(
             this IGlobalConfiguration configuration,
             TFilter filter)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            return configuration.Use(filter, x => GlobalJobFilters.Filters.Add(x));
        }

        public static IGlobalConfiguration UseDashboardMetric(
            this IGlobalConfiguration configuration,
             DashboardMetric metric)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (metric == null) throw new ArgumentNullException(nameof(metric));

            DashboardMetrics.AddMetric(metric);
            HomePage.Metrics.Add(metric);

            return configuration;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IGlobalConfiguration<T> Use<T>(
             this IGlobalConfiguration configuration, T entry,
             Action<T> entryAction)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            entryAction(entry);

            return new ConfigurationEntry<T>(entry);
        }

        private class ConfigurationEntry<T> : IGlobalConfiguration<T>
        {
            public ConfigurationEntry(T entry)
            {
                Entry = entry;
            }

            public T Entry { get; }
        }
    }
}

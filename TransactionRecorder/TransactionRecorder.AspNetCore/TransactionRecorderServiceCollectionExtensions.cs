using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Threading;
using TransactionRecorder.Core;

namespace TransactionRecorder
{
    public static class TransactionRecorderServiceCollectionExtensions
    {
        /// <summary>
        /// 是否經過初始化的變數
        /// 0: 未初始
        /// 1: ??
        /// </summary>
        private static int _initialized = 0;
        public static IServiceCollection AddTransactionRecorder(
            this IServiceCollection services,
            Action<IGlobalConfiguration> configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.TryAddSingleton(GetInitializedJobStorage);

            //services.TryAddSingleton(_ => GlobalConfiguration.Configuration);
            //services.TryAddSingleton(_ => JobActivator.Current);
            //services.TryAddSingleton(_ => DashboardRoutes.Routes);

            //services.TryAddSingleton<IJobFilterProvider>(_ => GlobalJobFilters.Filters);

            //services.TryAddSingleton<IBackgroundJobFactory>(x => new BackgroundJobFactory(
            //    x.GetRequiredService<IJobFilterProvider>()));

            //services.TryAddSingleton<IBackgroundJobStateChanger>(x => new BackgroundJobStateChanger(
            //    x.GetRequiredService<IJobFilterProvider>()));

            //services.TryAddSingleton<IBackgroundJobPerformer>(x => new BackgroundJobPerformer(
            //    x.GetRequiredService<IJobFilterProvider>(),
            //    x.GetRequiredService<JobActivator>()));

            //services.TryAddSingleton<IBackgroundJobClient>(x => new BackgroundJobClient(
            //    x.GetRequiredService<JobStorage>(),
            //    x.GetRequiredService<IBackgroundJobFactory>(),
            //    x.GetRequiredService<IBackgroundJobStateChanger>()));

            //services.TryAddSingleton<IRecurringJobManager>(x => new RecurringJobManager(
            //    x.GetRequiredService<JobStorage>(),
            //    x.GetRequiredService<IBackgroundJobFactory>()));

            services.TryAddSingleton<Action<IGlobalConfiguration>>(serviceProvider =>
            {
                return config =>
                {
                    if (Interlocked.CompareExchange(ref _initialized, 1, 0) != 0) return;

                    var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                    if (loggerFactory != null)
                    {
                        config.UseLogProvider(new AspNetCoreLogProvider(loggerFactory));
                    }

                    var scopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
                    if (scopeFactory != null)
                    {
                        config.UseActivator(new AspNetCoreJobActivator(scopeFactory));
                    }

                    configuration(config);
                };
            });

            return services;
        }
        private static JobStorage GetInitializedJobStorage(IServiceProvider serviceProvider)
        {
            // Non-initialized JobStorage instance may me asked to be resolved, when there 
            // were no calls to noth UseHangfireServer and UseHangfireDashboard methods, for
            // example, when application instance only use client methods.

            // To not to introduce almost empty `UseHangfireClient` method, we are using lazy 
            // initialization here, to run configuration action, when JobStorage instance is
            // being resolved as a result of IBackgroundJobClient interface resolution.

            if (Volatile.Read(ref _initialized) == 0)
            {
                var configurationInstance = serviceProvider.GetService<IGlobalConfiguration>();
                var configurationAction = serviceProvider.GetRequiredService<Action<IGlobalConfiguration>>();

                // This action is guarded against multiple calls itself, so Volatile.Read 
                // check is enough. In the worst case, only two additional services will
                // be resolved, that's not a problem.
                configurationAction(configurationInstance);
            }

            return JobStorage.Current;
        }
    }
}

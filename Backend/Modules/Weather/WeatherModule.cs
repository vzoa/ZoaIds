using Coravel.Scheduling.Schedule.Interfaces;
using ZoaIdsBackend.Common.Interfaces;
using ZoaIdsBackend.Modules.Weather.ScheduledJobs;

namespace ZoaIdsBackend.Modules.Weather;

public class WeatherModule : IServiceConfigurator, ISchedulerConfigurator
{
    public static string BaseUri { get; } = "/weather";

    public IServiceCollection AddServices(IServiceCollection serivces)
    {
        serivces.AddTransient<FetchAndStoreMetars>();
        return serivces;
    }

    public Action<IScheduler> ConfigureScheduler()
    {
        return scheduler =>
        {
            scheduler.Schedule<FetchAndStoreMetars>()
                .Cron("*/6 * * * *") // every 6 minutes
                .RunOnceAtStart();
        };
    }
}

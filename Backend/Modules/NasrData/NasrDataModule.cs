using Coravel.Scheduling.Schedule.Interfaces;
using ZoaIdsBackend.Common.Interfaces;
using ZoaIdsBackend.Modules.NasrData.ScheduledJobs;

namespace ZoaIdsBackend.Modules.NasrData;

public class NasrDataModule : IServiceConfigurator, ISchedulerConfigurator
{
    public IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddTransient<FetchAndStoreNasrData>();
        return services;
    }

    public Action<IScheduler> ConfigureScheduler()
    {
        var rnd = new Random();
        return scheduler =>
        {
            scheduler.Schedule<FetchAndStoreNasrData>()
                .DailyAt(9, rnd.Next(60))
                .RunOnceAtStart();
        };
    }
}

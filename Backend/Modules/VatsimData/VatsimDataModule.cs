using Coravel.Scheduling.Schedule.Interfaces;
using ZoaIdsBackend.Common.Interfaces;
using ZoaIdsBackend.Modules.VatsimData.Repositories;
using ZoaIdsBackend.Modules.VatsimData.ScheduledJobs;
using ZoaIdsBackend.Modules.VatsimData.Services;

namespace ZoaIdsBackend.Modules.VatsimData;

public class VatsimDataModule : IServiceConfigurator, ISchedulerConfigurator
{
    public static string BaseUri { get; } = "/vatsim";

    public IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddTransient<FetchAndStoreVatspyBoundaries>();
        services.AddTransient<DeleteOldVatsimSnapshots>();
        services.AddSingleton<VatsimDataRepositoryFactory>();
        services.AddHostedService<VatsimDataBackgroundService>();
        services.AddScoped<IVatsimDataRepository, CachedVatsimDataRepository>();
        return services;
    }

    public Action<IScheduler> ConfigureScheduler()
    {
        var rnd = new Random();
        return scheduler =>
        {
            scheduler.Schedule<FetchAndStoreVatspyBoundaries>()
                .DailyAt(9, rnd.Next(60))
                .RunOnceAtStart();

            scheduler
                .Schedule<DeleteOldVatsimSnapshots>()
                .Hourly()
                .RunOnceAtStart();
        };
    }
}

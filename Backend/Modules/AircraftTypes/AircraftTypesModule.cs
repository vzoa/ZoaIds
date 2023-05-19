using Coravel.Scheduling.Schedule.Interfaces;
using ZoaIdsBackend.Common.Interfaces;
using ZoaIdsBackend.Modules.AircraftTypes.ScheduledJobs;

namespace ZoaIdsBackend.Modules.AircraftTypes;

public class AircraftTypesModule : IServiceConfigurator, ISchedulerConfigurator
{
    public static string BaseUri { get; } = "/aircraft";

    public IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddTransient<FetchAndStoreAircraftInfo>();
        return services;
    }

    public Action<IScheduler> ConfigureScheduler()
    {
        var rnd = new Random();
        return scheduler =>
        {
            scheduler.Schedule<FetchAndStoreAircraftInfo>()
                .DailyAt(9, rnd.Next(60))
                .RunOnceAtStart();
        };
    }
}

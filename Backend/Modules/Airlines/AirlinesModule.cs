using Coravel.Scheduling.Schedule.Interfaces;
using ZoaIdsBackend.Common.Interfaces;
using ZoaIdsBackend.Modules.Airlines.ScheduledJobs;

namespace ZoaIdsBackend.Modules.Airlines;

public class AirlinesModule : IServiceConfigurator, ISchedulerConfigurator
{
    public static string BaseUri { get; } = "/airlines";

    public IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddTransient<FetchAndStoreAirlineIcao>();
        return services;
    }

    public Action<IScheduler> ConfigureScheduler()
    {
        var rnd = new Random();
        return scheduler =>
        {
            scheduler.Schedule<FetchAndStoreAirlineIcao>()
                .DailyAt(9, rnd.Next(60))
                .RunOnceAtStart();
        };
    }
}

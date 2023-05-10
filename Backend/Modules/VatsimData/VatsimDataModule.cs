using ZoaIdsBackend.Common.Interfaces;
using ZoaIdsBackend.Modules.VatsimData.Repositories;
using ZoaIdsBackend.Modules.VatsimData.Services;

namespace ZoaIdsBackend.Modules.VatsimData;

public class VatsimDataModule : IServiceConfigurator
{
    public static string BaseUri { get; } = "/vatsim";

    public IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddSingleton<VatsimDataRepositoryFactory>();
        services.AddHostedService<VatsimDataBackgroundService>();
        services.AddScoped<IVatsimDataRepository, CachedVatsimDataRepository>();
        return services;
    }
}

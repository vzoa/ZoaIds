using ZoaIdsBackend.Common.Interfaces;
using ZoaIdsBackend.Modules.VnasData.Services;

namespace ZoaIdsBackend.Modules.VatsimData;

public class VnasDataModule : IServiceConfigurator
{
    public IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddScoped<CachedVnasDataService>();
        return services;
    }
}

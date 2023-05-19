using ZoaIdsBackend.Common.Interfaces;
using ZoaIdsBackend.Modules.RunwayVisualRange.Services;

namespace ZoaIdsBackend.Modules.RunwayVisualRange;

public class RunwayVisualRangeModule : IServiceConfigurator
{
    public IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddHostedService<FaaRvrScraperBackgroundService>();
        return services;
    }
}
using ZoaIdsBackend.Common.Interfaces;
using ZoaIdsBackend.Modules.Routes.Services;

namespace ZoaIdsBackend.Modules.Routes;

public class RoutesModule : IServiceConfigurator
{
    public IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddTransient<FlightAwareRouteService>();
        return services;
    }
}

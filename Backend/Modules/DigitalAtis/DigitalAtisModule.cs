using ZoaIdsBackend.Common.Interfaces;
using ZoaIdsBackend.Modules.DigitalAtis.Services;

namespace ZoaIdsBackend.Modules.DigitalAtis;

public class DigitalAtisModule : IServiceConfigurator
{
    public static string BaseUri { get; } = "/datis";

    public IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddHostedService<DigitalAtisBackgroundService>();
        return services;
    }
}

using ZoaIdsBackend.Common.Interfaces;
using ZoaIdsBackend.Modules.Charts.Services;

namespace ZoaIdsBackend.Modules.Airlines;

public class ChartsModule : IServiceConfigurator
{
    public static string BaseUri { get; } = "/charts";

    public IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddTransient<AviationApiChartService>();
        return services;
    }
}

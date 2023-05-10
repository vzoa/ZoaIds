using FastEndpoints;
using Microsoft.Extensions.Options;
using ZoaIdsBackend.Modules.Routes.Models;
using ZoaIdsBackend.Modules.Routes.Services;

namespace ZoaIdsBackend.Modules.Routes.Endpoints;

public class RouteRequest
{
    public string DepartureIcaoId { get; set; } = string.Empty;
    public string ArrivalIcaoId { get; set; } = string.Empty;
}

public class GetRealWorldRoutes : Endpoint<RouteRequest, AirportPairRouteSummary>
{
    private readonly FlightAwareRouteService _routeService;
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public GetRealWorldRoutes(FlightAwareRouteService flightAwareRouteService, IOptionsMonitor<AppSettings> appSettings)
    {
        _routeService = flightAwareRouteService;
        _appSettings = appSettings;
    }

    public override void Configure()
    {
        Get("/routes/{DepartureIcaoId}/{ArrivalIcaoId}");
        ResponseCache(_appSettings.CurrentValue.CacheTtls.FlightAwareRoutes);
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(RouteRequest request, CancellationToken c)
    {
        var route = await _routeService.FetchRoutesAsync(request.DepartureIcaoId.ToUpper(), request.ArrivalIcaoId.ToUpper());
        await SendAsync(route);
    }
}

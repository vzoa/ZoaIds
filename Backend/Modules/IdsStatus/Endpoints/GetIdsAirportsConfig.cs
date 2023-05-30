using FastEndpoints;
using Microsoft.Extensions.Options;

namespace ZoaIdsBackend.Modules.IdsStatus.Endpoints;

public class AirportsDefinition
{
    public ICollection<string> Bravo { get; set; } = new List<string>();
    public ICollection<string> Charlie { get; set; } = new List<string>();
    public ICollection<string> Delta { get; set; } = new List<string>();
}

public class GetIdsAirportsConfig : EndpointWithoutRequest<AirportsDefinition>
{
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public GetIdsAirportsConfig(IOptionsMonitor<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public override void Configure()
    {
        Get(IdsStatusModule.BaseUri + "/airports");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var response = new AirportsDefinition
        {
            Bravo = _appSettings.CurrentValue.ArtccAirports.Bravos,
            Charlie = _appSettings.CurrentValue.ArtccAirports.Charlies,
            Delta = _appSettings.CurrentValue.ArtccAirports.Deltas
        };

        await SendAsync(response);
    }
}

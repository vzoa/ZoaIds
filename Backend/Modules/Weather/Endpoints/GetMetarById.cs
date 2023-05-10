using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.Weather.Models;

namespace ZoaIdsBackend.Modules.Weather.Endpoints;

public class SingleStationRequest
{
    public string Id { get; set; } = string.Empty;
}

public class GetMetarById : Endpoint<SingleStationRequest, Metar>
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public GetMetarById(IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public override void Configure()
    {
        Get(WeatherModule.BaseUri + "/metars/{@id}", x => new { x.Id });
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(SingleStationRequest request, CancellationToken c)
    {
        using var db = await _contextFactory.CreateDbContextAsync(c);
        var returnMetar = await db.Metars.FindAsync(request.Id.ToUpper());

        if (returnMetar is not null)
        {
            await SendAsync(returnMetar);
        }
        else
        {
            await SendNotFoundAsync();
        }
    }
}

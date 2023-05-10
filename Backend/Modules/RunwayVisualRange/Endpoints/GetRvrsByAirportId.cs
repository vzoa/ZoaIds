using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.RunwayVisualRange.Models;

namespace ZoaIdsBackend.Modules.VatsimData.Endpoints;

public class AirportRvrRequest
{
    public string FaaId { get; set; } = string.Empty;
}

public class GetRvrsByAirportId : Endpoint<AirportRvrRequest, ICollection<RvrObservation>>
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public GetRvrsByAirportId(IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public override void Configure()
    {
        Get("/airports/{FaaId}/rvrs", "/rvrs/{FaaId}");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(AirportRvrRequest request, CancellationToken c)
    {
        using var db = await _contextFactory.CreateDbContextAsync(c);
        var rvr = await db.RvrObservations.AsNoTracking().Where(r => r.AirportFaaId == request.FaaId.ToUpper()).ToListAsync();

        if (rvr is null || rvr.Count == 0)
        {
            await SendNotFoundAsync();
        }
        else
        {
            await SendAsync(rvr);
        }
    }
}

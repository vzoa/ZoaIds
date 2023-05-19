using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.NasrData.Models;

namespace ZoaIdsBackend.Modules.NasrData.Endpoints;

public class ArtccRequest
{
    public string Id { get; set; } = string.Empty;
}


public class GetAirportsWithinArtcc : Endpoint<ArtccRequest, IEnumerable<Airport>>
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public GetAirportsWithinArtcc(IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public override void Configure()
    {
        Get("/artccs/{@id}/airports", x => new { x.Id });
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(ArtccRequest request, CancellationToken c)
    {
        using var db = await _contextFactory.CreateDbContextAsync(c);
        var airports = db.Airports.AsNoTracking().Where(a => a.Artcc == request.Id.ToUpper());
        await SendAsync(airports);
    }
}

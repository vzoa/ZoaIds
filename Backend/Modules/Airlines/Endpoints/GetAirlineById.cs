using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.Airlines.Models;

namespace ZoaIdsBackend.Modules.Airlines.Endpoints;

public class AirlineIdRequest
{
    public string IcaoId { get; set; } = string.Empty;
}

public class GetAirlineById : Endpoint<AirlineIdRequest, Airline>
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public GetAirlineById(IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public override void Configure()
    {
        Get("/airlines/{@id}", x => new { x.IcaoId });
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(AirlineIdRequest request, CancellationToken c)
    {
        using var db = await _contextFactory.CreateDbContextAsync();
        var airline = await db.Airlines.FindAsync(request.IcaoId.ToUpper());
        if (airline is null)
        {
            await SendNotFoundAsync();
        }
        else
        {
            await SendOkAsync(airline);
        }
    }
}

using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.DigitalAtis.Models;

namespace ZoaIdsBackend.Modules.DigitalAtis.Endpoints;

public class DigitalAtisRequest
{
    public string IcaoId { get; set; } = string.Empty;
}


public class GetDigitalAtisById : Endpoint<DigitalAtisRequest, IEnumerable<Atis>>
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public GetDigitalAtisById(IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public override void Configure()
    {
        Get(DigitalAtisModule.BaseUri + "/{@id}", x => new { x.IcaoId });
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(DigitalAtisRequest request, CancellationToken c)
    {
        using var db = await _contextFactory.CreateDbContextAsync(c);
        var atises = db.Atises.Where(a => a.IcaoId == request.IcaoId.ToUpper());
        
        if (atises.Any())
        {
            await SendAsync(atises);
        }
        else
        {
            await SendNotFoundAsync();
        }
    }
}

using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.DigitalAtis.Models;

namespace ZoaIdsBackend.Modules.DigitalAtis.Endpoints;

public class GetAllDigitalAtis : EndpointWithoutRequest<ICollection<Atis>>
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public GetAllDigitalAtis(IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public override void Configure()
    {
        Get(DigitalAtisModule.BaseUri);
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        using var db = await _contextFactory.CreateDbContextAsync(c);
        var list = await db.Atises.AsNoTracking().ToListAsync(c) ?? new List<Atis>();
        await SendAsync(list);
    }
}
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ZoaIdsBackend.Common;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.VatsimData.Models;
using ZoaIdsBackend.Modules.VatsimData.Repositories;

namespace ZoaIdsBackend.Modules.VatsimData.Endpoints;

public class ArtccPilotsRequest
{
    public string Id { get; set; } = string.Empty;
}

public class GetPilotsWithinArtcc : Endpoint<ArtccPilotsRequest, IEnumerable<VatsimJsonPilot>>
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly IVatsimDataRepository _repository;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public GetPilotsWithinArtcc(IDbContextFactory<ZoaIdsContext> contextFactory, IVatsimDataRepository repository)
    {
        _contextFactory = contextFactory;
        _repository = repository;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new PolygonJsonConverter(), new GeoCoordinateJsonConverter() }
        };
    }

    public override void Configure()
    {
        Get(VatsimDataModule.BaseUri + "/artccs/{@id}/pilots", x => new { x.Id });
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(ArtccPilotsRequest request, CancellationToken c)
    {
        // Get requested ARTCC from DB
        using var db = await _contextFactory.CreateDbContextAsync(c);
        var artccs = db.Artccs.Where(a => a.Id == request.Id.ToUpper());
        if (!artccs.Any())
        {
            await SendNotFoundAsync();
        }

        var artccsList = await artccs.ToListAsync();
        var polygons = artccsList.SelectMany(a => JsonSerializer.Deserialize<List<Polygon>>(a.SerializedBoundingPolygons, _jsonSerializerOptions));
        var vatsimData = await _repository.GetLatestDataAsync();
        var returnPilots = new List<VatsimJsonPilot>();
        foreach (var pilot in vatsimData.Pilots)
        {
            var containsList = polygons.Select(p => p.Contains(new GeoCoordinate((double)pilot.Latitude!, (double)pilot.Longitude!)));
            if (containsList.Contains(true))
            {
                returnPilots.Add(pilot);
            }
        }

        await SendAsync(returnPilots);
    }
}

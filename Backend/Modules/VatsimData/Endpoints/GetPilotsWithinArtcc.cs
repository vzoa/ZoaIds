using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
    private readonly IMemoryCache _cache;

    public GetPilotsWithinArtcc(IDbContextFactory<ZoaIdsContext> contextFactory, IVatsimDataRepository repository, IMemoryCache cache)
    {
        _contextFactory = contextFactory;
        _repository = repository;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = { new PolygonJsonConverter(), new GeoCoordinateJsonConverter() }
        };
        _cache = cache;
    }

    public override void Configure()
    {
        Get(VatsimDataModule.BaseUri + "/artccs/{@id}/pilots", x => new { x.Id });
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(ArtccPilotsRequest request, CancellationToken c)
    {
        var vatsimData = await _repository.GetLatestDataAsync();

        // Check if we have a cached result from the current VATSIM datafeed timestamp. If so return early
        if (_cache.TryGetValue<(string timestamp, IEnumerable<VatsimJsonPilot> pilots)>(MakeCacheKey(request.Id.ToUpper()), out var cachedResult))
        {
            if (vatsimData is not null && vatsimData.General.Update == cachedResult.timestamp)
            {
                await SendAsync(cachedResult.pilots);
                return;
            }
        }

        // Otherwise, get new result start with requested ARTCC from DB. Return early if not ARTCC found with id.
        using var db = await _contextFactory.CreateDbContextAsync(c);
        var artccs = db.Artccs.Where(a => a.Id == request.Id.ToUpper());
        if (!artccs.Any())
        {
            await SendNotFoundAsync();
            return;
        }

        var artccsList = await artccs.ToListAsync();
        var polygons = artccsList.SelectMany(a => JsonSerializer.Deserialize<List<Polygon>>(a.SerializedBoundingPolygons, _jsonSerializerOptions));
        var returnPilots = new List<VatsimJsonPilot>();
        foreach (var pilot in vatsimData.Pilots)
        {
            var containsList = polygons.Select(p => p.Contains(new GeoCoordinate((double)pilot.Latitude!, (double)pilot.Longitude!)));
            if (containsList.Contains(true))
            {
                returnPilots.Add(pilot);
            }
        }

        // Cache new result and return
        _cache.Set<(string, IEnumerable<VatsimJsonPilot>)>(MakeCacheKey(request.Id.ToUpper()), (vatsimData.General.Update, returnPilots));
        await SendAsync(returnPilots);
    }

    private static string MakeCacheKey(string artccID) => $"PilotsWithin:{artccID}";
}

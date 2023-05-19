namespace ZoaIdsBackend.Modules.VatsimData.ScheduledJobs;

using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZoaIdsBackend.Common;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.VatsimData.Models;

public class FetchAndStoreVatspyBoundaries : IInvocable
{
    private readonly ILogger<FetchAndStoreVatspyBoundaries> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly HttpClient _httpClient;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public FetchAndStoreVatspyBoundaries(ILogger<FetchAndStoreVatspyBoundaries> logger, IDbContextFactory<ZoaIdsContext> contextFactory, HttpClient httpClient, IOptionsMonitor<AppSettings> appSettings)
    {
        _logger = logger;
        _httpClient = httpClient;
        _contextFactory = contextFactory;
        _appSettings = appSettings;
        _jsonSerializerOptions = new JsonSerializerOptions()
        {
            Converters = { new PolygonJsonConverter(), new GeoCoordinateJsonConverter() }
        };
    }

    public async Task Invoke()
    {
        // Fetch VatSpy Boundaries GeoJSON file
        var url = _appSettings.CurrentValue.Urls.ArtccBoundariesGeoJson;
        using var responseStream = await _httpClient.GetStreamAsync(url);
        _logger.LogInformation("Fetched VatSpy Boundaries GeoJSON file from: {url}", url);

        // Deserialize JSON to objects
        var parsedArtccs = Enumerable.Empty<Artcc>();
        try
        {
            // Parse all the GeoJSON features in ARTCC objects
            var options = new JsonSerializerOptions { NumberHandling = JsonNumberHandling.AllowReadingFromString };
            var featureCollection = JsonSerializer.Deserialize<VatspyBoundariesGeoJson.Root>(responseStream, options);
            parsedArtccs = featureCollection.Features.Select(NewArtccFromFeature);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while deserializing JSON: {ex}", ex);
            return;
        }

        // Clear DB and add new ARTCCs
        try
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            var numDeleted = await db.Artccs.ExecuteDeleteAsync();
            _logger.LogInformation("Deleted {num} ARTCCs from database to be re-added", numDeleted);

            await db.AddRangeAsync(parsedArtccs);
            await db.SaveChangesAsync();
            _logger.LogInformation("Added {num} ARTCCs to database", parsedArtccs.Count());
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Error while modifying ARTCCs in database: {ex}", ex.ToString());
        }
    }

    private static IEnumerable<GeoCoordinate> FlattenCoordinates(List<List<List<double>>> jsonPoly)
    {
        return jsonPoly.SelectMany(coordsListInner => coordsListInner.Select(coords => new GeoCoordinate(coords[1], coords[0])));
    }

    private Artcc NewArtccFromFeature(VatspyBoundariesGeoJson.Feature feature)
    {
        var polys = feature.Geometry.Coordinates.Select(c => new Polygon(FlattenCoordinates(c)));
        return new Artcc
        {
            Id = feature.Properties.Id,
            Region = feature.Properties.Region,
            Division = feature.Properties.Division,
            IsOceanic = feature.Properties.IsOceanic,
            SerializedBoundingPolygons = JsonSerializer.Serialize(polys, _jsonSerializerOptions)
        };
    }
}

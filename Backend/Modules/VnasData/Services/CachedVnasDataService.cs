using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZoaIdsBackend.Modules.VnasData.Models;

namespace ZoaIdsBackend.Modules.VnasData.Services;

public class CachedVnasDataService
{
    private readonly IMemoryCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private IOptionsMonitor<AppSettings> _appSettings;
    private readonly JsonSerializerOptions _jsonOptions;

    public CachedVnasDataService(IMemoryCache cache, IHttpClientFactory httpClientFactory, IOptionsMonitor<AppSettings> appSettings)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _jsonOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };
        _appSettings = appSettings;
    }

    public async Task<IEnumerable<Facility>?> GetArtccFacilities(string artccId, CancellationToken c = default)
    {
        if (_cache.TryGetValue<IEnumerable<Facility>>(MakeCacheKey(artccId), out var cached))
        {
            return cached;
        }
        
        var httpClient = _httpClientFactory.CreateClient();
        var jsonRoot = await httpClient.GetFromJsonAsync<VnasApiRoot>($"{_appSettings.CurrentValue.Urls.VnasApiEndpoint}/artccs/{artccId.ToUpper()}", _jsonOptions, c);

        if (jsonRoot is null)
        {
            return Enumerable.Empty<Facility>();
        }

        var returnFacilities = new List<Facility>();
        var queue = new Queue<Facility>();
        queue.Enqueue(jsonRoot.Facility);

        while (queue.Count > 0)
        {
            var facility = queue.Dequeue();
            returnFacilities.Add(facility);
            facility.ChildFacilities.ForEach(c => queue.Enqueue(c));
        }

        var expiration = DateTimeOffset.UtcNow.AddSeconds(_appSettings.CurrentValue.CacheTtls.VnasData);
        _cache.Set(MakeCacheKey(artccId), returnFacilities, expiration);
        return returnFacilities;
    }

    private static string MakeCacheKey(string id) => $"VnasData:{id}";
}

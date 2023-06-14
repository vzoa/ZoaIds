using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.VatsimData.Models;

namespace ZoaIdsBackend.Modules.VatsimData.Repositories;

public class CachedVatsimDataRepository : IVatsimDataRepository
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly IMemoryCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    public const string LatestSnaphotKey = "LatestVatsimSnapshot";

    public CachedVatsimDataRepository(IDbContextFactory<ZoaIdsContext> contextFactory, IMemoryCache memoryCache, IHttpClientFactory httpClientFactory, IOptionsMonitor<AppSettings> appSettings)
    {
        _contextFactory = contextFactory;
        _cache = memoryCache;
        _httpClientFactory = httpClientFactory;
        _appSettings = appSettings;
    }

    public async Task<VatsimJsonRoot?> GetLatestDataAsync(CancellationToken c = default)
    {
        var latestSnapshot = await GetLatestSnapshotAsync(c);
        return latestSnapshot switch
        {
            null => null,
            _ => JsonSerializer.Deserialize<VatsimJsonRoot>(latestSnapshot.RawJson)
        };
    }

    public async Task<VatsimSnapshot?> GetLatestSnapshotAsync(CancellationToken c = default)
    {
        if (_cache.TryGetValue<VatsimSnapshot>(LatestSnaphotKey, out var snapshot))
        {
            return snapshot;
        }

        using var db = await _contextFactory.CreateDbContextAsync(c);
        return await db.VatsimSnapshots
            .AsNoTracking()
            .OrderByDescending(x => x.Time)
            .FirstOrDefaultAsync(c);
    }

    public async Task<ICollection<VatsimSnapshot>> GetAllSnapshotsAsync(CancellationToken c = default)
    {
        using var db = await _contextFactory.CreateDbContextAsync(c);
        return await db.VatsimSnapshots.AsNoTracking().OrderByDescending(x => x.Time).ToListAsync(c);
    }

    public async Task<int> SaveLatestSnapshotAsync(VatsimSnapshot snapshot, CancellationToken c = default)
    {
        using var db = await _contextFactory.CreateDbContextAsync(c);
        await db.VatsimSnapshots.AddAsync(snapshot, c);
        var n = await db.SaveChangesAsync(c);
        _cache.Set(LatestSnaphotKey, snapshot);
        return n;
    }

    public async Task<int> DeleteAllSnapshotsBefore(DateTime cutoff, CancellationToken c = default)
    {
        using var db = await _contextFactory.CreateDbContextAsync(c);
        var numDeleted = await db.VatsimSnapshots.Where(s => s.Time < cutoff).ExecuteDeleteAsync(c);
        if (!db.VatsimSnapshots.Any())
        {
            _cache.Remove(LatestSnaphotKey);
        }

        return numDeleted;
    }

    public async Task<VatsimUserDetails?> GetUserDetailsAsync(int id, CancellationToken c = default)
    {
        if (_cache.TryGetValue<VatsimUserDetails>(MakeDetailsCacheKey(id), out var details))
        {
            return details;
        }

        var httpClient = _httpClientFactory.CreateClient();
        var fetchedDetails = await httpClient.GetFromJsonAsync<VatsimUserDetails>($"{_appSettings.CurrentValue.Urls.VatsimApiEndpoint}/members/{id}", c);
        var expiration = DateTimeOffset.UtcNow.AddSeconds(_appSettings.CurrentValue.CacheTtls.VatsimUserStats);
        _cache.Set(MakeDetailsCacheKey(id), fetchedDetails, expiration);
        return fetchedDetails;
    }

    public async Task<VatsimUserStats?> GetUserStatsAsync(int id, CancellationToken c = default)
    {
        if (_cache.TryGetValue<VatsimUserStats?>(MakeStatsCacheKey(id), out var stats))
        {
            return stats;
        }

        var httpClient = _httpClientFactory.CreateClient();
        var statsTask = await httpClient.GetAsync($"{_appSettings.CurrentValue.Urls.VatsimApiEndpoint}/members/{id}/stats", c);
        if (!statsTask.IsSuccessStatusCode)
        {
            _cache.Set<VatsimUserStats?>(MakeDetailsCacheKey(id), null, DateTimeOffset.UtcNow.AddSeconds(_appSettings.CurrentValue.CacheTtls.VatsimUserStats));
            return null;
        }

        var fetchedStats = await statsTask.Content.ReadFromJsonAsync<VatsimUserStats>(cancellationToken: c);
        var expiration = DateTimeOffset.UtcNow.AddSeconds(_appSettings.CurrentValue.CacheTtls.VatsimUserStats);
        _cache.Set(MakeDetailsCacheKey(id), fetchedStats, expiration);
        return fetchedStats;
    }

    private string MakeDetailsCacheKey(int id) => $"VatsimUserDetails:{id}";
    private string MakeStatsCacheKey(int id) => $"VatsimUserStats:{id}";
}

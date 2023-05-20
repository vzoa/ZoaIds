using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.VatsimData.Models;

namespace ZoaIdsBackend.Modules.VatsimData.Repositories;

public class CachedVatsimDataRepository : IVatsimDataRepository
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly IMemoryCache _cache;
    public const string LatestSnaphotKey = "LatestVatsimSnapshot";

    public CachedVatsimDataRepository(IDbContextFactory<ZoaIdsContext> contextFactory, IMemoryCache memoryCache)
    {
        _contextFactory = contextFactory;
        _cache = memoryCache;
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
        else
        {
            using var db = await _contextFactory.CreateDbContextAsync(c);
            return await db.VatsimSnapshots
                .AsNoTracking()
                .OrderByDescending(x => x.Time)
                .FirstOrDefaultAsync(c);
        }
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
}

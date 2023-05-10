//using Microsoft.EntityFrameworkCore;
//using System.Text.Json;
//using System.Threading;
//using ZoaIdsBackend.Data;
//using ZoaIdsBackend.Modules.VatsimData.Models;

//namespace ZoaIdsBackend.Modules.VatsimData.Repositories;

//public static class DbSetExtensions
//{
//    public static async Task<VatsimJsonRoot?> GetLatestDataAsync(this DbSet<VatsimSnapshot> dbSet, CancellationToken c = default)
//    {
//        var latestSnapshot = await dbSet.GetLatestSnapshotAsync(c);

//        return latestSnapshot switch
//        {
//            null => null,
//            _ => JsonSerializer.Deserialize<VatsimJsonRoot>(latestSnapshot.RawJson)
//        };
//    }

//    public static async Task<VatsimSnapshot?> GetLatestSnapshotAsync(this DbSet<VatsimSnapshot> dbSet, CancellationToken c = default)
//    {
//        return await dbSet
//            .AsNoTracking()
//            .OrderByDescending(x => x.Time)
//            .FirstOrDefaultAsync(c);
//    }
//}

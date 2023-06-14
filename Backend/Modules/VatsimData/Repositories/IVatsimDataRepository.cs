using ZoaIdsBackend.Modules.VatsimData.Models;

namespace ZoaIdsBackend.Modules.VatsimData.Repositories;

public interface IVatsimDataRepository
{
    public Task<VatsimJsonRoot?> GetLatestDataAsync(CancellationToken c = default);
    public Task<VatsimSnapshot?> GetLatestSnapshotAsync(CancellationToken c = default);
    public Task<int> SaveLatestSnapshotAsync(VatsimSnapshot snapshot, CancellationToken c = default);
    public Task<ICollection<VatsimSnapshot>> GetAllSnapshotsAsync(CancellationToken c = default);
    public Task<int> DeleteAllSnapshotsBefore(DateTime cutoff, CancellationToken c = default);
    public Task<VatsimUserDetails?> GetUserDetailsAsync(int id, CancellationToken c = default);
    public Task<VatsimUserStats?> GetUserStatsAsync(int id, CancellationToken c = default);
}

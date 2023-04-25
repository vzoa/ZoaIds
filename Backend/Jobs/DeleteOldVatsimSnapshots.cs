using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Jobs;

public class DeleteOldVatsimSnapshots : IInvocable
{
    private readonly ILogger<DeleteOldVatsimSnapshots> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly TimeSpan _keepForDuration;

    public DeleteOldVatsimSnapshots(ILogger<DeleteOldVatsimSnapshots> logger, IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _keepForDuration = TimeSpan.FromHours(Constants.VatsimDataKeepForfHours);
    }

    public async Task Invoke()
    {
        try
        {
            using var db = await _contextFactory.CreateDbContextAsync();
            var cutoff = DateTime.UtcNow - _keepForDuration;
            var numDeleted = await db.VatsimSnapshots.Where(s => s.Time < cutoff).ExecuteDeleteAsync();
            _logger.LogInformation("Deleted {n} old VATSIM datafeed snapshots", numDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error deleting old VATSIM datafeed snapshots: {ex}", ex.ToString());
        }
    }
}
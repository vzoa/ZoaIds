using Coravel.Invocable;
using Microsoft.Extensions.Options;
using ZoaIdsBackend.Modules.VatsimData.Repositories;

namespace ZoaIdsBackend.Modules.VatsimData.ScheduledJobs;

public class DeleteOldVatsimSnapshots : IInvocable
{
    private readonly ILogger<DeleteOldVatsimSnapshots> _logger;
    private readonly IVatsimDataRepository _vatsimDataRepository;
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public DeleteOldVatsimSnapshots(ILogger<DeleteOldVatsimSnapshots> logger, IVatsimDataRepository vatsimDataRepository, IOptionsMonitor<AppSettings> appSettings)
    {
        _logger = logger;
        _vatsimDataRepository = vatsimDataRepository;
        _appSettings = appSettings;
    }

    public async Task Invoke()
    {
        try
        {
            var cutoff = DateTime.UtcNow - TimeSpan.FromHours(_appSettings.CurrentValue.VatsimDataKeepForfHours);
            var numDeleted = await _vatsimDataRepository.DeleteAllSnapshotsBefore(cutoff);
            _logger.LogInformation("Deleted {n} old VATSIM datafeed snapshots", numDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error deleting old VATSIM datafeed snapshots: {ex}", ex.ToString());
        }
    }
}

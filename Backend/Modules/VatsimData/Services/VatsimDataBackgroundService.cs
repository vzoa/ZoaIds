using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;
using ZoaIdsBackend.Modules.VatsimData.Models;
using ZoaIdsBackend.Modules.VatsimData.Repositories;

namespace ZoaIdsBackend.Modules.VatsimData.Services;

public class VatsimDataBackgroundService : BackgroundService
{
    private readonly ILogger<VatsimDataBackgroundService> _logger;
    private readonly VatsimDataRepositoryFactory _repositoryFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private int _delaySeconds;
    private readonly Stopwatch _stopwatch;

    public VatsimDataBackgroundService(ILogger<VatsimDataBackgroundService> logger,
                                       VatsimDataRepositoryFactory repositoryFactory,
                                       IHttpClientFactory httpClientFactory,
                                       IOptionsMonitor<AppSettings> appSettings)
    {
        _logger = logger;
        _repositoryFactory = repositoryFactory;
        _httpClientFactory = httpClientFactory;
        _appSettings = appSettings;
        _delaySeconds = appSettings.CurrentValue.VatsimDatafeedRefreshSeconds;
        _stopwatch = new Stopwatch();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool firstLoop = true;

        while (!stoppingToken.IsCancellationRequested)
        {
            // Delay at beginning of the loop, but skip if first time through the loop
            if (!firstLoop)
            {
                // Stop the stopwatch and use elapsed loop time to adjust delay time so that
                // loop-start to loop-start time is consistent
                _stopwatch.Stop();
                var adjustedDelay = TimeSpan.FromSeconds(_delaySeconds) - _stopwatch.Elapsed;
                _logger.LogInformation("Pausing VATSIM Data Worker for {delay} seconds", adjustedDelay.TotalSeconds.ToString());
                await Task.Delay(adjustedDelay, stoppingToken);
            }

            firstLoop = false;
            _stopwatch.Reset();
            _stopwatch.Start();

            // Attempt to fetch the datafeed JSON file. If we have an error, retry in 1 second
            var url = _appSettings.CurrentValue.Urls.VatsimDatafeed;
            _logger.LogInformation("Fetching VATSIM datafeed from {url} at: {time}", url, DateTime.UtcNow);
            Stream stream;
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                stream = await httpClient.GetStreamAsync(url, stoppingToken);
                _logger.LogInformation("Successfully fetched VATSIM datafeed");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching VATSIM datafeed: {error}", ex.ToString());
                _delaySeconds = 1;
                continue;
            }

            // Attempt to parse JSON into a new Vatsim Snapshot object.
            VatsimSnapshot newSnapshot;
            try
            {
                using var json = await JsonDocument.ParseAsync(stream, cancellationToken: stoppingToken);
                var root = json.RootElement;
                var generalArray = root.GetProperty("general");
                newSnapshot = new()
                {
                    Id = long.Parse(generalArray.GetProperty("update").GetString()),
                    Time = generalArray.GetProperty("update_timestamp").GetDateTime(),
                    RawJson = root.ToString()
                };
                stream.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error parsing VATSIM JSON data: {error}", ex.ToString());
                continue;
            }

            // Check if this is a new snapshot. If it is, save to DB. If not, retry in 1 second.
            try
            {
                var repository = _repositoryFactory.CreateVatsimDataRepository();
                var previousSnapshot = await repository.GetLatestSnapshotAsync(stoppingToken);

                if (previousSnapshot is null || newSnapshot.Id > previousSnapshot.Id)
                {
                    await repository.SaveLatestSnapshotAsync(newSnapshot, stoppingToken);
                    _delaySeconds = _appSettings.CurrentValue.VatsimDatafeedRefreshSeconds;
                    _logger.LogInformation("Successfully wrote VATSIM datafeed to database with timestamp: {time}", newSnapshot.Time);
                }
                else
                {
                    _logger.LogInformation("Found a duplicate VATSIM datafeed with timestamp: {time}", newSnapshot.Time);
                    _delaySeconds = 1;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error saving VATSIM data to database: {error}", ex.ToString());
                continue;
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Services;

public class VatsimDataWorker : BackgroundService
{
    private readonly ILogger<VatsimDataWorker> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly HttpClient _httpClient;
    private int _delaySeconds;
    private readonly Stopwatch _stopwatch;

    public VatsimDataWorker(ILogger<VatsimDataWorker> logger, IDbContextFactory<ZoaIdsContext> contextFactory, HttpClient httpClient)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(Constants.Urls.VatsimDatafeedBase);
        _delaySeconds = Constants.VatsimDatafeedUpdateFrequencySeconds;
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
            _logger.LogInformation("Fetching VATSIM datafeed at: {time}", DateTime.UtcNow);
            Stream stream;
            try
            {
                stream = await _httpClient.GetStreamAsync("vatsim-data.json", stoppingToken);
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
                using var db = await _contextFactory.CreateDbContextAsync(stoppingToken);

                var previousSnapshot = await db.VatsimSnapshots
                    .AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync(stoppingToken);

                if (previousSnapshot is null || newSnapshot.Id > previousSnapshot.Id)
                {
                    await db.VatsimSnapshots.AddAsync(newSnapshot, stoppingToken);
                    await db.SaveChangesAsync(stoppingToken);

                    _delaySeconds = 15;
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

using Coravel.Invocable;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using ZoaIdsBackend.Common;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.Airlines.Models;

namespace ZoaIdsBackend.Modules.Airlines.ScheduledJobs;

public class FetchAndStoreAirlineIcao : IInvocable
{
    private readonly ILogger<FetchAndStoreAirlineIcao> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public FetchAndStoreAirlineIcao(ILogger<FetchAndStoreAirlineIcao> logger, IDbContextFactory<ZoaIdsContext> contextFactory, IHttpClientFactory httpClientFactory, IOptionsMonitor<AppSettings> appSettings)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _contextFactory = contextFactory;
        _appSettings = appSettings;
    }

    public async Task Invoke()
    {
        //TODO ADD TRY CATCH and logging

        // Setup reads and DB context
        //try
        //{

        //}
        //catch (Exception ex)
        //{
        //	_logger.LogError("Error while trying to fetch and read Aircraft ICAO csv: {ex}", ex.ToString());
        //	return;
        //}

        // Setup reads and DB context
        var url = _appSettings.CurrentValue.Urls.AirlinesCsv;
        var client = _httpClientFactory.CreateClient();
        using var responseStream = await client.GetStreamAsync(url);
        using var reader = new StreamReader(responseStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        using var db = await _contextFactory.CreateDbContextAsync();
        _logger.LogInformation("Fetched Airline ICAO CSV file from: {url}", url);

        // Read the first row and get the id string
        csv.Read();
        var csvUpdated = csv.GetField(0);

        // Query database to get the most recent successful fetch of the NASR data
        var lastUpdated = await db.CompletedJobs
            .AsNoTracking()
            .Where(j => j.Caller == GetType().Name && j.JobKey == "fetch" && j.Status == ApplicationJobRecord.ExitStatus.Success)
            .OrderByDescending(j => j.Time)
            .FirstOrDefaultAsync();

        // Return if there are no updates to CSV data
        if (csvUpdated == lastUpdated?.JobValue)
        {
            _logger.LogInformation("No updates to Airline ICAO data, exiting fetch task");
            return;
        }

        // Read the rest of the file as objects
        csv.Context.RegisterClassMap<CsvAirlineMap>();
        var records = csv.GetRecords<Airline>().ToArray();

        // Delete old data
        var numDeleted = await db.Airlines.ExecuteDeleteAsync();
        _logger.LogInformation("Deleted {num} records from Airlines database", numDeleted);

        // Add new data
        await db.Airlines.AddRangeAsync(records);
        await db.SaveChangesAsync();
        _logger.LogInformation("Added {num} records to Airlines database", records.Length);

        // Save to table of successful jobs
        var job = new ApplicationJobRecord()
        {
            Caller = GetType().Name,
            Time = DateTime.UtcNow,
            JobKey = "fetch",
            JobValue = csvUpdated ?? string.Empty,
            Status = ApplicationJobRecord.ExitStatus.Success
        };
        await db.CompletedJobs.AddAsync(job);
        await db.SaveChangesAsync();
    }

    private class CsvAirlineMap : ClassMap<Airline>
    {
        public CsvAirlineMap()
        {
            Map(m => m.IcaoId).Name("3Ltr");
            Map(m => m.Name).Name("Company");
            Map(m => m.Callsign).Name("Telephony");
            Map(m => m.Country).Name("Country");
        }
    }
}

using Coravel.Invocable;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using ZoaIdsBackend.Common;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.AircraftTypes.Models;

namespace ZoaIdsBackend.Modules.AircraftTypes.ScheduledJobs;

public class FetchAndStoreAircraftInfo : IInvocable
{
    private readonly ILogger<FetchAndStoreAircraftInfo> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly HttpClient _httpClient;
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public FetchAndStoreAircraftInfo(ILogger<FetchAndStoreAircraftInfo> logger, IDbContextFactory<ZoaIdsContext> contextFactory, HttpClient httpClient, IOptionsMonitor<AppSettings> appSettings)
    {
        _logger = logger;
        _httpClient = httpClient;
        _contextFactory = contextFactory;
        _appSettings = appSettings;
    }

    public async Task Invoke()
    {

        await Task.Delay(1000 * 10);

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
        using var responseStream = await _httpClient.GetStreamAsync(_appSettings.CurrentValue.Urls.AircraftCsv);
        using var reader = new StreamReader(responseStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        using var db = await _contextFactory.CreateDbContextAsync();
        _logger.LogInformation("Fetched Aircraft ICAO CSV file from: {url}", _appSettings.CurrentValue.Urls.AircraftCsv);

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
            _logger.LogInformation("No updates to Aircraft ICAO data, exiting fetch task");
            return;
        }

        // Read the rest of the file as objects
        csv.Context.RegisterClassMap<CsvAircraftMap>();
        var records = csv.GetRecords<AircraftType>().ToArray();

        // Delete old data
        var numDeleted = await db.AircraftTypes.ExecuteDeleteAsync();
        _logger.LogInformation("Deleted {num} records from Aircraft database", numDeleted);

        // Add new data
        await db.AircraftTypes.AddRangeAsync(records);
        await db.SaveChangesAsync();
        _logger.LogInformation("Added {num} records to Aircraft database", records.Length);

        // Save to table of successful jobs
        var job = new ApplicationJobRecord()
        {
            Caller = GetType().Name,
            Time = DateTime.UtcNow,
            JobKey = "fetch",
            JobValue = csvUpdated,
            Status = ApplicationJobRecord.ExitStatus.Success
        };
        await db.CompletedJobs.AddAsync(job);
        await db.SaveChangesAsync();
    }

    private class CsvAircraftMap : ClassMap<AircraftType>
    {
        public CsvAircraftMap()
        {
            Map(m => m.IcaoId).Name("Type Designator");
            Map(m => m.Manufacturer).Name("Manufacturer");
            Map(m => m.Model).Name("Model");
            Map(m => m.Class).Name("Description");
            Map(m => m.EngineType).Name("Engine Type");
            Map(m => m.EngineCount).Name("Engine Count");
            Map(m => m.IcaoWakeTurbulenceCategory).Name("WTC");
            Map(m => m.FaaEngineNumberType).Name("Engine Number-Type");
            Map(m => m.FaaWeightClass).Name("FAA Weight Class");
            Map(m => m.ConslidatedWakeTurbulenceCategory).Name("CWT");
            Map(m => m.SameRunwaySeparationCategory).Name("SRS");
            Map(m => m.LandAndHoldShortGroup).Name("LAHSO");
        }
    }
}

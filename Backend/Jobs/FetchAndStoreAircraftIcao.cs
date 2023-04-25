using Coravel.Invocable;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Jobs;

public class FetchAndStoreAircraftIcao : IInvocable
{
    private readonly ILogger<FetchAndStoreAircraftIcao> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly HttpClient _httpClient;

    public FetchAndStoreAircraftIcao(ILogger<FetchAndStoreAircraftIcao> logger, IDbContextFactory<ZoaIdsContext> contextFactory, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        _contextFactory = contextFactory;
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
        using var responseStream = await _httpClient.GetStreamAsync(Constants.Urls.AircraftIcaoCsv);
        using var reader = new StreamReader(responseStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        using var db = await _contextFactory.CreateDbContextAsync();
        _logger.LogInformation("Fetched Aircraft ICAO CSV file from: {url}", Constants.Urls.AircraftIcaoCsv);

        // Read the first row and get the id string
        csv.Read();
        var csvUpdated = csv.GetField(0);

        // Query database to get the most recent successful fetch of the NASR data
        var lastUpdated = await db.CompletedJobs
            .AsNoTracking()
            .Where(j => j.Caller == GetType().Name && j.JobKey == "fetch" && j.ExitStatus == JobExitStatus.Success)
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
        var records = csv.GetRecords<AircraftTypeInfo>().ToArray();

        // Delete old data
        var numDeleted = await db.AircraftTypes.ExecuteDeleteAsync();
        _logger.LogInformation("Deleted {num} records from Aircraft database", numDeleted);

        // Add new data
        await db.AircraftTypes.AddRangeAsync(records);
        await db.SaveChangesAsync();
        _logger.LogInformation("Added {num} records to Aircraft database", records.Length);

        // Save to table of successful jobs
        var job = new ApplicationJob()
        {
            Caller = GetType().Name,
            Time = DateTime.UtcNow,
            JobKey = "fetch",
            JobValue = csvUpdated,
            ExitStatus = JobExitStatus.Success
        };
        await db.CompletedJobs.AddAsync(job);
        await db.SaveChangesAsync();
    }

    private class CsvAircraftMap : ClassMap<AircraftTypeInfo>
    {
        public CsvAircraftMap()
        {
            Map(m => m.IcaoId).Name("Type Designator");
            Map(m => m.Manufacturer).Name("Manufacturer");
            Map(m => m.Model).Name("Model");
            Map(m => m.Description).Name("Description");
            Map(m => m.EngineType).Name("Engine Type");
            Map(m => m.EngineCount).Name("Engine Count");
            Map(m => m.FaaWakeTurbulenceCategory).Name("WTC");
        }
    }
}

using Coravel.Invocable;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Jobs;

public class FetchAndStoreAirlineIcao : IInvocable
{
    private readonly ILogger<FetchAndStoreAirlineIcao> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly HttpClient _httpClient;

    public FetchAndStoreAirlineIcao(ILogger<FetchAndStoreAirlineIcao> logger, IDbContextFactory<ZoaIdsContext> contextFactory, HttpClient httpClient)
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
        using var responseStream = await _httpClient.GetStreamAsync(Constants.Urls.AirlinesIcaoCsv);
        using var reader = new StreamReader(responseStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        using var db = await _contextFactory.CreateDbContextAsync();
        _logger.LogInformation("Fetched Airline ICAO CSV file from: {url}", Constants.Urls.AirlinesIcaoCsv);

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
            _logger.LogInformation("No updates to Airline ICAO data, exiting fetch task");
            return;
        }

        // Read the rest of the file as objects
        csv.Context.RegisterClassMap<CsvAirlineMap>();
        var records = csv.GetRecords<AirlineInfo>().ToArray();

        // Delete old data
        var numDeleted = await db.Airlines.ExecuteDeleteAsync();
        _logger.LogInformation("Deleted {num} records from Airlines database", numDeleted);

        // Add new data
        await db.Airlines.AddRangeAsync(records);
        await db.SaveChangesAsync();
        _logger.LogInformation("Added {num} records to Airlines database", records.Length);

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

    private class CsvAirlineMap : ClassMap<AirlineInfo>
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

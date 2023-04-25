using Coravel.Invocable;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO.Compression;
using System.Xml;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Jobs;

public class FetchAndStoreNasrData : IInvocable
{
    private readonly ILogger<FetchAndStoreNasrData> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly HttpClient _httpClient;

    public FetchAndStoreNasrData(ILogger<FetchAndStoreNasrData> logger, IDbContextFactory<ZoaIdsContext> contextFactory, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        _contextFactory = contextFactory;
    }

    public async Task Invoke()
    {
        // Call FAA API to get the url for the latest NASR zip file
        var zipUrl = await GetNasrZipUrl();
        _logger.LogInformation("Fetched current NASR subscription zip file: {url}", zipUrl);

        // Query database to get the most recent successful fetch of the NASR data
        try
        {

            using var db = await _contextFactory.CreateDbContextAsync();
            var lastFetch = await db.CompletedJobs
                .AsNoTracking()
                .Where(j => j.Caller == typeof(FetchAndStoreNasrData).Name && j.JobKey == "fetch" && j.ExitStatus == JobExitStatus.Success)
                .OrderByDescending(j => j.Time)
                .FirstOrDefaultAsync();

            // Return early if the subscription hasn't changed
            if (lastFetch is not null && lastFetch.JobValue == zipUrl)
            {
                _logger.LogInformation("NASR data has not been updated as of: {time}", DateTime.UtcNow);
                return;
            }

            _logger.LogInformation("Found new NASR file: {url}", zipUrl);

            // Open the Base data zip file
            using var stream = await _httpClient.GetStreamAsync(zipUrl);
            using var nasrArchive = new ZipArchive(stream);

            // Open the Airports data zip file inside
            var airportEntry = nasrArchive.Entries.Where(x => x.Name.Contains("APT_CSV.zip")).FirstOrDefault();
            using var airportArchive = new ZipArchive(airportEntry.Open());

            // Parse the individual zip files inside the Airports data zip file
            var airportRecords = await ParseZippedCsvToObjects(airportArchive, "APT_BASE.csv", ParseCsvRowToAirport);
            var runwayRecords = await ParseZippedCsvToObjects(airportArchive, "APT_RWY.csv", ParseCsvRowToRunway);
            var runwayEndRecords = await ParseZippedCsvToObjects(airportArchive, "APT_RWY_END.csv", ParseCsvRowToRunwayEnd);

            // Add Runways to Airports
            foreach (var runway in runwayRecords.Values)
            {
                var id = runway.AirportFaaId;
                try
                {
                    var x = airportRecords[id];
                    airportRecords[id].Runways.Add(runway);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            // Add Runway Ends to Runways
            foreach (var runwayEnd in runwayEndRecords.Values)
            {
                var runwayId = runwayEnd.RunwayName;
                var airportId = runwayEnd.AirportFaaId;
                var runway = airportRecords[airportId].Runways.Where(r => r.Name == runwayId).SingleOrDefault();
                // Test line
                if (runway is null)
                {
                    continue;
                }
                runway?.Ends.Add(runwayEnd);
            }

            // Delete all old airports and save new airports data to DB
            var numDeleted = await db.Airports.ExecuteDeleteAsync();
            _logger.LogInformation("Deleted {numDeleted} airports from NASR database", numDeleted);
            await db.Airports.AddRangeAsync(airportRecords.Values);
            await db.SaveChangesAsync();
            _logger.LogInformation("Added {numAdded} airports to NASR database", airportRecords.Values.Count);


            // Add the successful fetch into the metadata jobs table so that we can reference in future
            var job = new ApplicationJob()
            {
                Caller = typeof(FetchAndStoreNasrData).Name,
                Time = DateTime.UtcNow,
                JobKey = "fetch",
                JobValue = zipUrl,
                ExitStatus = JobExitStatus.Success
            };
            await db.CompletedJobs.AddAsync(job);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error: {error message}", ex.ToString());
        }
    }

    private async Task<string> GetNasrZipUrl()
    {
        // API request to get current edition
        using var apiStream = await _httpClient.GetStreamAsync(Constants.Urls.NasrApiEndpoint);

        // Load XML response
        using var xmlReader = new XmlTextReader(apiStream) { Namespaces = false };
        var doc = new XmlDocument();
        doc.Load(xmlReader);

        // Select via XPath and return
        var urlNode = doc.SelectSingleNode(".//edition[@editionName='CURRENT']/product/@url");
        return urlNode.Value;
    }

    private static AirportType StringToAirportType(string code)
    {
        return code switch
        {
            "A" => AirportType.Airport,
            "B" => AirportType.Ballonport,
            "C" => AirportType.SeaplaneBase,
            "G" => AirportType.Gliderport,
            "H" => AirportType.Heliport,
            "U" => AirportType.Ultralight,
            _ => throw new NotImplementedException()
        };
    }

    private static async Task<IDictionary<string, T>> ParseZippedCsvToObjects<T>(ZipArchive zip, string filename, Func<CsvReader, (string, T)> lineParserFunc)
    {
        var entry = zip.GetEntry(filename);
        using var stream = new StreamReader(entry.Open());
        using var reader = new CsvReader(stream, CultureInfo.InvariantCulture);

        var returnDict = new Dictionary<string, T>();
        await reader.ReadAsync();
        reader.ReadHeader();

        while (await reader.ReadAsync())
        {
            var parsedTuple = lineParserFunc(reader);
            returnDict.TryAdd(parsedTuple.Item1, parsedTuple.Item2);
        }

        return returnDict;
    }

    private static (string, Airport) ParseCsvRowToAirport(CsvReader csv)
    {
        var airport = new Airport
        {
            FaaId = csv.GetField("ARPT_ID"),
            IcaoId = string.IsNullOrEmpty(csv.GetField("ICAO_ID")) ? null : csv.GetField("ICAO_ID"),
            Name = csv.GetField("ARPT_NAME"),
            Elevation = csv.GetField<double>("ELEV"),
            Artcc = csv.GetField("RESP_ARTCC_ID"),
            Location = new GeoCoordinate(csv.GetField<double>("LAT_DECIMAL"), csv.GetField<double>("LONG_DECIMAL")),
            Type = StringToAirportType(csv.GetField("SITE_TYPE_CODE"))
        };

        if (string.IsNullOrEmpty(csv.GetField("MAG_HEMIS")) || string.IsNullOrEmpty(csv.GetField("MAG_VARN")))
        {
            airport.TrueToMagneticDelta = null;
        }
        else
        {
            airport.TrueToMagneticDelta = (csv.GetField("MAG_HEMIS") == "E" ? -1 : 1) * csv.GetField<int>("MAG_VARN");
        }

        return (airport.FaaId, airport);
    }

    private static (string, Runway) ParseCsvRowToRunway(CsvReader csv)
    {
        var runway = new Runway
        {
            Name = csv.GetField("RWY_ID"),
            Length = csv.GetField<int>("RWY_LEN"),
            Ends = new List<RunwayEnd>(),
            AirportFaaId = csv.GetField("ARPT_ID")
        };
        var key = $"{runway.AirportFaaId}-{runway.Name}";
        return (key, runway);
    }

    private static (string, RunwayEnd) ParseCsvRowToRunwayEnd(CsvReader csv)
    {
        var runwayEnd = new RunwayEnd
        {
            Name = csv.GetField("RWY_END_ID"),
            RunwayName = csv.GetField("RWY_ID"),
            AirportFaaId = csv.GetField("ARPT_ID")
        };

        // Not all runway ends have elevation data, so check null
        var elevString = csv.GetField("RWY_END_ELEV");
        runwayEnd.EndElevation = string.IsNullOrEmpty(elevString) ? null : double.Parse(elevString);

        // Not all runway ends have TDZ elevation data, so check null
        var tdzString = csv.GetField("TDZ_ELEV");
        runwayEnd.TdzElevation = string.IsNullOrEmpty(tdzString) ? null : double.Parse(tdzString);

        // Not all runway ends have true alignment data, so check null
        var trueHeadingString = csv.GetField("TRUE_ALIGNMENT");
        runwayEnd.TrueHeading = string.IsNullOrEmpty(trueHeadingString) ? null : int.Parse(trueHeadingString);

        var key = $"{runwayEnd.AirportFaaId}-{runwayEnd.RunwayName}-{runwayEnd.Name}";
        return (key, runwayEnd);
    }
}









//    private static async Task<ICollection<Airport>> AirportBaseCsvToAirports(CsvReader csv)
//    {
//        var returnAirports = new List<Airport>();

//        await csv.ReadAsync();
//        csv.ReadHeader();
//        while (await csv.ReadAsync())
//        {
//            var airport = ParseCsvRowToAirport(csv);
//            returnAirports.Add(airport);
//        }

//        return returnAirports;
//    }

//    private static async Task<ICollection<Runway>> RunwayBaseCsvToRunways(CsvReader csv)
//    {
//        var returnRunways = new List<Runway>();

//        await csv.ReadAsync();
//        csv.ReadHeader();
//        while (await csv.ReadAsync())
//        {
//            var runway = ParseCsvRowToRunway(csv);
//            returnRunways.Add(runway);
//        }

//        return returnRunways;
//    }

//    private static async Task<ICollection<RunwayEnd>> RunwayEndsCsvToRunwayEnds(CsvReader csv)
//    {
//        var returnRunwayEnds = new List<RunwayEnd>();

//        await csv.ReadAsync();
//        csv.ReadHeader();

//        while (await csv.ReadAsync())
//        {
//            var runwayEnd = ParseCsvRowToRunwayEnd(csv);
//            returnRunwayEnds.Add(runwayEnd);
//        }

//        return returnRunwayEnds;
//    }
//}
using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Xml.Serialization;
using ZoaIdsBackend.Common;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.Weather.Models;

namespace ZoaIdsBackend.Modules.Weather.ScheduledJobs;

public class FetchAndStoreMetars : IInvocable
{
    private readonly ILogger<FetchAndStoreMetars> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly HttpClient _httpClient;
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public FetchAndStoreMetars(ILogger<FetchAndStoreMetars> logger, IDbContextFactory<ZoaIdsContext> contextFactory, HttpClient httpClient, IOptionsMonitor<AppSettings> appSettings)
    {
        _logger = logger;
        _httpClient = httpClient;
        _contextFactory = contextFactory;
        _appSettings = appSettings;
    }

    public async Task Invoke()
    {
        //TODO ADD TRY CATCH and logging around the fetch and parse section

        await Task.Delay(7 * 1000);

        // Fetch and parse XML
        using var responseStream = await _httpClient.GetStreamAsync(_appSettings.CurrentValue.Urls.MetarsXml);
        var serializer = new XmlSerializer(typeof(MetarApiXmlResponse));
        var xml = (MetarApiXmlResponse)serializer.Deserialize(responseStream);

        // Todo need some way to check if the file has been updated, or just run every 6 minutes instead
        using var db = await _contextFactory.CreateDbContextAsync();
        var existingMetars = await db.Metars.AsNoTracking().ToDictionaryAsync(m => m.StationId, m => m);
        var newMetars = new Dictionary<string, Metar>();
        var updatedMetars = new Dictionary<string, Metar>();

        foreach (var metar in xml.data.METAR)
        {
            // Parse XML class into our domain model
            var newMetar = ParseFromXmlMetar(metar);

            // Check if this Metar exists in our DB, and if so, check if it is newer than existing record
            if (existingMetars.TryGetValue(newMetar.StationId, out var existing))
            {
                if (newMetar.ObservationTime > existing.ObservationTime)
                {
                    // If it exists and is newer, add to our to-be-updated Metar dictionary
                    // Need to guard with TryAdd as sometimes the XML can have duplicate Station IDs
                    if (!updatedMetars.TryAdd(newMetar.StationId, newMetar))
                    {
                        _logger.LogInformation("Unable to add METAR for {id} to updated METARs list due to duplicate key", newMetar.StationId);
                    }
                }
            }
            else
            {
                // If it doesn't exist in our DB, add to our new Metar dictionary
                if (!newMetars.TryAdd(newMetar.StationId, newMetar))
                {
                    _logger.LogInformation("Unable to add METAR for {id} to new METARs list due to duplicate key", newMetar.StationId);
                }
            }
        }

        try
        {
            // Delete metars from DB where will be inserting updates
            var numDeleted = await db.Metars.Where(m => updatedMetars.Keys.Contains(m.StationId)).ExecuteDeleteAsync();
            _logger.LogInformation("Deleted {num} metars from database to be updated", numDeleted);

            // Add the updated mtars where we deleted old records
            await db.Metars.AddRangeAsync(updatedMetars.Values);
            await db.SaveChangesAsync();
            _logger.LogInformation("Added {numAdded} metars to database as updates", updatedMetars.Values.Count);

            // Add metars for station IDs that don't currently exist in DB
            await db.Metars.AddRangeAsync(newMetars.Values);
            await db.SaveChangesAsync();
            _logger.LogInformation("Added {numAdded} metars to database as new station IDs", newMetars.Values.Count);
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Error while updating METARs in database: {ex}", ex.ToString());
        }
    }

    private Metar ParseFromXmlMetar(METAR metar)
    {
        // Parse the XML-derived Metar object into our domain Metar object
        var newMetar = new Metar
        {
            StationId = metar.station_id,
            RawText = metar.raw_text,
            ObservationTime = DateTime.ParseExact(metar.observation_time, "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal),
            WeatherString = metar.wx_string,
            AltimeterInHg = metar.altim_in_hgSpecified ? metar.altim_in_hg : null,
            TempC = metar.temp_cSpecified ? metar.temp_c : null,
            DewPointC = metar.dewpoint_cSpecified ? metar.dewpoint_c : null,
            VisibilityMi = metar.visibility_statute_miSpecified ? metar.visibility_statute_mi : null,
            SeaLevelPressureMb = metar.sea_level_pressure_mbSpecified ? metar.sea_level_pressure_mb : null,
            VerticalVisibilityFt = metar.vert_vis_ftSpecified ? metar.vert_vis_ft : null,
            Type = (MetarType)Enum.Parse(typeof(MetarType), metar.metar_type)
        };

        // Parse the sky conditions / covers and add as SkyCoverObservation objects to domain Metar object
        if (metar.sky_condition is not null && metar.sky_condition.Length > 0)
        {
            newMetar.SkyCovers = new List<SkyCoverObservation>();
            foreach (var cover in metar.sky_condition)
            {
                var obs = new SkyCoverObservation
                {
                    Type = (CloudCoverType)Enum.Parse(typeof(CloudCoverType), cover.sky_cover),
                    BaseFtAgl = cover.cloud_base_ft_aglSpecified ? cover.cloud_base_ft_agl : null
                };
                newMetar.SkyCovers.Add(obs);
            }
        }

        // Parse the wind and add as WindObservation object to domain Metar object
        if (metar.wind_dir_degreesSpecified)
        {
            newMetar.Wind = new WindObservation
            {
                DirectionTrueDegrees = metar.wind_dir_degrees,
                SpeedKnots = metar.wind_speed_kt,
                GustKnots = metar.wind_gust_ktSpecified ? metar.wind_gust_kt : null
            };
        }

        // Parse the location and add as Coordinate object to Metar object
        if (metar.longitudeSpecified && metar.latitudeSpecified)
        {
            newMetar.Location = new GeoCoordinate(metar.latitude, metar.longitude);
        }

        return newMetar;
    }
}
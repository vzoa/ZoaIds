﻿using AngleSharp.Html.Parser;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using ZoaIdsBackend.Common;
using ZoaIdsBackend.Modules.Routes.Models;

namespace ZoaIdsBackend.Modules.Routes.Services;

public partial class FlightAwareRouteService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly IMemoryCache _cache;

    public FlightAwareRouteService(IHttpClientFactory httpClientFactory, IOptionsMonitor<AppSettings> appSettings, IMemoryCache cache)
    {
        _httpClientFactory = httpClientFactory;
        _appSettings = appSettings;
        _cache = cache;
    }

    public async Task<AirportPairRouteSummary> FetchRoutesAsync(string departureIcao, string arrivalIcao)
    {
        // First check cache and return early if we have cached result
        if (_cache.TryGetValue<AirportPairRouteSummary>(MakeCacheKey(departureIcao, arrivalIcao), out var cachedResult))
        {
            return cachedResult!;
        }

        // If not, fetch result from FlightAware
        try
        {
            // Setup return object
            var returnRoute = new AirportPairRouteSummary(departureIcao, arrivalIcao);

            // Open FlightAware IFR routing page
            var client = _httpClientFactory.CreateClient();
            using var stream = await client.GetStreamAsync(MakeUrl(departureIcao, arrivalIcao));
            var parser = new HtmlParser();
            using var document = await parser.ParseDocumentAsync(stream);

            // Set up a temporary lookup dict for processing tables into objects
            var routesDict = new Dictionary<string, FlightRouteSummary>();

            // Initial parsing to select main tables and rows
            var tables = document.QuerySelectorAll("table.prettyTable.fullWidth");
            var summaryTable = tables[0];
            var flightsTable = tables[1];
            var summaryRows = summaryTable?.QuerySelectorAll("tr");
            var flightRows = flightsTable?.QuerySelectorAll("tr");

            // Iterate through Route Summary table and create new FlightRouteSummary for each row
            for (int i = 0; i < summaryRows.Length; i++)
            {
                // Ignore the first two rows which are table headers; every row after that is a data row
                if (i <= 1) { continue; }

                var tds = summaryRows[i].QuerySelectorAll("td");
                var newRouteSummary = new FlightRouteSummary
                {
                    RouteFrequency = int.Parse(tds[0].TextContent),
                    DepartureIcaoId = tds[1].TextContent,
                    ArrivalIcaoId = tds[2].TextContent,
                    MinAltitude = TryParseMinAltitude(tds[3].TextContent, out var minAlt) ? minAlt : null,
                    MaxAltitude = TryParseMaxAltitude(tds[3].TextContent, out var maxAlt) ? maxAlt : null,
                    Route = tds[4].TextContent,
                    DistanceMi = TryParseDistance(tds[5].TextContent, out var distance) ? distance : null,
                    Flights = new List<RealWorldFlight>()
                };
                returnRoute.FlightRouteSummaries.Add(newRouteSummary);
                routesDict[newRouteSummary.Route] = newRouteSummary; // Temp lookup dict
            }

            // Iterate through itemized routes table and create new RealWorldFlight for each row.
            // Use the lookup dict to add to correct RouteSummary
            for (int i = 0; i < flightRows.Length; i++)
            {
                // Ignore the first two rows which are table headers; every row after that is a data row
                if (i <= 1) { continue; }

                var tds = flightRows[i].QuerySelectorAll("td");
                var newFlight = new RealWorldFlight
                {
                    Callsign = tds[1].TextContent.Trim(),
                    DepartureIcaoId = tds[1].TextContent,
                    ArrivalIcaoId = tds[2].TextContent,
                    AircraftIcaoId = tds[4].TextContent,
                    Altitude = Helpers.TryParseAltitude(tds[5].TextContent, out var alt) ? alt : null,
                    Route = tds[6].TextContent,
                    Distance = TryParseDistance(tds[7].TextContent, out var distance) ? distance : null
                };

                // Add to associated RouteSummary
                if (routesDict.TryGetValue(newFlight.Route, out var route))
                {
                    route.Flights.Add(newFlight);
                }
            }

            // Cache result before returning
            var expiration = DateTimeOffset.UtcNow.AddSeconds(_appSettings.CurrentValue.CacheTtls.FlightAwareRoutes);
            _cache.Set(MakeCacheKey(departureIcao, arrivalIcao), returnRoute, expiration);

            return returnRoute;
        }
        catch (HttpRequestException e)
        {
            throw e;
        }
    }

    private string MakeUrl(string departureIcao, string arrivalIcao)// => $"{appSettings.CurrentValue.Urls.FlightAwareIfrRouteBase}origin={departureIcao}&destination{arrivalIcao}";
    {
        return _appSettings.CurrentValue.Urls.FlightAwareIfrRouteBase + @"origin=" + departureIcao + @"&destination=" + arrivalIcao;
    }

    private static bool TryParseMinAltitude(string altitudeRange, out int minAltitude)
    {
        var match = AltitudeRangeRegex().Match(altitudeRange);
        var parseString = match.Success ? match.Groups[1].Value : altitudeRange;
        return Helpers.TryParseAltitude(parseString, out minAltitude);
    }

    private static bool TryParseMaxAltitude(string altitudeRange, out int maxAltitude)
    {
        var match = AltitudeRangeRegex().Match(altitudeRange);
        var parseString = match.Success ? match.Groups[2].Value : altitudeRange;
        return Helpers.TryParseAltitude(parseString, out maxAltitude);
    }

    private static bool TryParseDistance(string distanceStr, out int distance)
    {
        var match = DistanceRegex().Match(distanceStr);
        var parseString = match.Success ? match.Groups[1].Value.Replace(",", string.Empty) : distanceStr;
        return int.TryParse(parseString, out distance);
    }

    private static (string, string) MakeCacheKey(string departureIcao, string arrivalIcao)
    {
        return ($"FlightAwareDeparture:{departureIcao.ToUpper()}", $"FlightAwareArrival:{arrivalIcao.ToUpper()}");
    }

    [GeneratedRegex("([\\s\\S]+) - ([\\s\\S]+)")]
    private static partial Regex AltitudeRangeRegex();

    [GeneratedRegex("([0-9,]+) sm")]
    private static partial Regex DistanceRegex();
}
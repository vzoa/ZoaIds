using AngleSharp.Html.Parser;
using System.Text.RegularExpressions;
using ZoaIdsBackend.Services;

namespace ZoaIds.Server.Services;

public partial class FlightAwareRouteService : IRouteSummaryService
{
    private readonly HttpClient _httpClient;

    public FlightAwareRouteService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<RealWorldRouting> FetchRoutesAsync(string departureIcao, string arrivalIcao)
    {
        try
        {
            // Setup return object
            var returnRoute = new RealWorldRouting(departureIcao, arrivalIcao);

            // Open FlightAware IFR routing page
            using var stream = await _httpClient.GetStreamAsync(MakeUrl(departureIcao, arrivalIcao));
            var parser = new HtmlParser();
            using var document = await parser.ParseDocumentAsync(stream);

            // Set up a temporary lookup dict for processing tables into objects
            var routesDict = new Dictionary<string, RouteSummary>();

            // Initial parsing to select main tables and rows
            var tables = document.QuerySelectorAll("table.prettyTable.fullWidth");
            var summaryTable = tables[0];
            var flightsTable = tables[1];
            var summaryRows = summaryTable?.QuerySelectorAll("tr");
            var flightRows = flightsTable?.QuerySelectorAll("tr");

            // Iterate through Route Summary table and create new RouteSummary for each row
            for (int i = 0; i < summaryRows.Length; i++)
            {
                // Ignore the first two rows which are table headers; every row after that is a data row
                if (i <= 1)
                {
                    continue;
                }

                var tds = summaryRows[i].QuerySelectorAll("td");
                var newRouteSummary = new RouteSummary
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
                returnRoute.RouteSummaries.Add(newRouteSummary);
                routesDict[newRouteSummary.Route] = newRouteSummary; // Temp lookup dict
            }

            // Iterate through itemized routes table and create new RealWorldFlight for each row.
            // Use the lookup dict to add to correct RouteSummary
            for (int i = 0; i < flightRows.Length; i++)
            {
                // Ignore the first two rows which are table headers; every row after that is a data row
                if (i <= 1)
                {
                    continue;
                }

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

            return returnRoute;
        }
        catch (HttpRequestException e)
        {
            throw e;
        }
    }

    private static string MakeUrl(string departureIcao, string arrivalIcao)
    {
        return Constants.Urls.FlightAwareIfrRouteBase + @"origin=" + departureIcao + @"&destination=" + arrivalIcao;
    }

    private static bool TryParseMinAltitude(string altitudeRange, out int minAltitude)
    {
        var match = AltitudeRangeRegex().Match(altitudeRange);
        if (match.Success)
        {
            var minAltitudeString = match.Groups[1].Value;
            return Helpers.TryParseAltitude(minAltitudeString, out minAltitude);
        }
        else
        {
            return Helpers.TryParseAltitude(altitudeRange, out minAltitude);
        }
    }

    private static bool TryParseMaxAltitude(string altitudeRange, out int maxAltitude)
    {
        var match = AltitudeRangeRegex().Match(altitudeRange);
        if (match.Success)
        {
            var maxAltitudeString = match.Groups[2].Value;
            return Helpers.TryParseAltitude(maxAltitudeString, out maxAltitude);
        }
        else
        {
            return Helpers.TryParseAltitude(altitudeRange, out maxAltitude);
        }
    }

    private static bool TryParseDistance(string distanceStr, out int distance)
    {
        var match = DistanceRegex().Match(distanceStr);
        if (match.Success)
        {
            var numString = match.Groups[1].Value.Replace(",", string.Empty);
            return int.TryParse(numString, out distance);
        }
        else
        {
            return int.TryParse(distanceStr, out distance);
        }
    }

    [GeneratedRegex("([\\s\\S]+) - ([\\s\\S]+)")]
    private static partial Regex AltitudeRangeRegex();

    [GeneratedRegex("([0-9,]+) sm")]
    private static partial Regex DistanceRegex();
}

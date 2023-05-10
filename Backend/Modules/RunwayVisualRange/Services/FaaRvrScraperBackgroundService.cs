using AngleSharp.Html.Parser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.RunwayVisualRange.Models;

namespace ZoaIdsBackend.Modules.RunwayVisualRange.Services;

public partial class FaaRvrScraperBackgroundService : BackgroundService
{
    private readonly ILogger<FaaRvrScraperBackgroundService> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public FaaRvrScraperBackgroundService(ILogger<FaaRvrScraperBackgroundService> logger, IDbContextFactory<ZoaIdsContext> contextFactory, IHttpClientFactory httpClientFactory, IOptionsMonitor<AppSettings> appSettings)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _httpClientFactory = httpClientFactory;
        _appSettings = appSettings;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            {
                // Open FAA RVR airport lookup page
                var httpClient = _httpClientFactory.CreateClient();
                using var stream = await httpClient.GetStreamAsync(_appSettings.CurrentValue.Urls.FaaRvrAirportLookup, stoppingToken);
                var parser = new HtmlParser();
                using var document = await parser.ParseDocumentAsync(stream);

                // Find all the links for airports and filter only those we care about in ZOA
                var airports = _appSettings.CurrentValue.ArtccAirports.All.Select(a => (a.StartsWith("K") && a.Length == 4) ? a[1..] : a);
                var linkElements = document.QuerySelectorAll("tr > td > a");
                var foundLinks = linkElements
                    .Where(e => airports.Contains(e.TextContent, StringComparer.OrdinalIgnoreCase))
                    .Select(e => (Id: e.TextContent, Url: e.GetAttribute("href")));

                // For each found link, open and parse RVR
                var tasks = new List<Task>();
                foreach (var (id, url) in foundLinks)
                {
                    var t = FetchRvrObservationAsync(id, url, stoppingToken, httpClient);
                    tasks.Add(t);
                }
                await Task.WhenAll(tasks);

                // Get the results from all of the completed tasks and store in DB
                var db = await _contextFactory.CreateDbContextAsync(stoppingToken);
                foreach (var task in tasks)
                {
                    var (id, list) = ((Task<(string, List<RvrObservation>)>)task).Result;

                    // Delete all existing RVR observations for this airport
                    var numDeleted = await db.RvrObservations.Where(r => r.AirportFaaId == id).ExecuteDeleteAsync(stoppingToken);
                    if (numDeleted > 0)
                    {
                        _logger.LogInformation("Deleted {numDeleted} RVR observations for {id}", numDeleted, id);
                    }

                    // Add all the new observations
                    await db.RvrObservations.AddRangeAsync(list, stoppingToken);
                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Added {num} RVR observations for {id}", list.Count, id);
                }
            }

            await Task.Delay(1000 * _appSettings.CurrentValue.RvrRefreshSeconds, stoppingToken);
        }
    }

    private async Task<(string, List<RvrObservation>)> FetchRvrObservationAsync(string airportFaaId, string url, CancellationToken stoppingToken, HttpClient? httpClient = null)
    {
        httpClient ??= _httpClientFactory.CreateClient();
        using var stream = await httpClient.GetStreamAsync(_appSettings.CurrentValue.Urls.FaaRvrBase + url, stoppingToken);
        var parser = new HtmlParser();
        using var document = await parser.ParseDocumentAsync(stream);

        var rows = document.QuerySelectorAll("font > table > tbody > tr").ToList();
        rows.RemoveAt(0); // First row is a header, so remove it

        var returnList = new List<RvrObservation>();
        foreach (var row in rows)
        {
            var th = row.QuerySelector("th");
            var tds = row.QuerySelectorAll("td");
            var newObs = new RvrObservation
            {
                AirportFaaId = airportFaaId,
                RunwayEndName = th.TextContent,
                Touchdown = ParseDistance(tds[0].TextContent),
                TouchdownTrend = ParseTrend(tds[0].TextContent),
                Midpoint = ParseDistance(tds[1].TextContent),
                MidpointTrend = ParseTrend(tds[1].TextContent),
                Rollout = ParseDistance(tds[2].TextContent),
                RolloutTrend = ParseTrend(tds[2].TextContent),
                EdgeLightSetting = ParseLightSetting(tds[3].TextContent),
                CenterlineLightSetting = ParseLightSetting(tds[4].TextContent)
            };
            returnList.Add(newObs);
        }
        return (airportFaaId, returnList);
    }

    private static int? ParseDistance(string text)
    {
        text = text.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }
        var match = NumberRegex().Match(text);
        return match.Success ? int.Parse(match.Groups[0].Value) : null;
    }

    private static RvrObservation.RvrTrend? ParseTrend(string text)
    {
        text = text.Trim();
        return text switch
        {
            string s when string.IsNullOrEmpty(s) => null,
            string s when s.Contains('▲') => RvrObservation.RvrTrend.Increasing,
            string s when s.Contains('▼') => RvrObservation.RvrTrend.Decreasing,
            _ => RvrObservation.RvrTrend.Steady
        };
    }

    private static int? ParseLightSetting(string text)
    {
        text = text.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }
        else
        {
            return int.TryParse(text, out var lightSetting) ? lightSetting : null;
        }
    }

    [GeneratedRegex("[0-9]+")]
    private static partial Regex NumberRegex();
}

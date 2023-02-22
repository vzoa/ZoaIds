using AngleSharp.Html.Parser;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using ZoaIds.Server.Data;
using ZoaIds.Shared.Models;

namespace ZoaIds.Server.Services
{
	public partial class RvrWorker : BackgroundService
	{
		private readonly ILogger<RvrWorker> _logger;
		private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
		private readonly HttpClient _httpClient;

		public RvrWorker(ILogger<RvrWorker> logger, IDbContextFactory<ZoaIdsContext> contextFactory, HttpClient httpClient)
		{
			_logger = logger;
			_contextFactory = contextFactory;
			_httpClient = httpClient;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			// Create a timer that fires every 2 min (default setting). Each loop will fetch and store RVRs
			using var timer = new PeriodicTimer(TimeSpan.FromSeconds(Constants.RvrUpdateIntervalSeconds));
			while(!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
			{
				// Open FAA RVR airport lookup page
				using var stream = await _httpClient.GetStreamAsync(Constants.Urls.FaaRvrAirportLookup, stoppingToken);
				var parser = new HtmlParser();
				using var document = await parser.ParseDocumentAsync(stream);

				// Find all the links for airports and filter only those we care about in ZOA
				var linkElements = document.QuerySelectorAll("tr > td > a");
				var foundLinks = linkElements
					.Where(e => Constants.RvrAirportIds.Contains(e.TextContent))
					.Select(e => (Id: e.TextContent, Url: e.GetAttribute("href")));

				// For each found link, open and parse RVR
				var tasks = new List<Task>();
				foreach (var (id, url) in foundLinks)
				{
					var t = FetchRvrObservationAsync(id, url, stoppingToken);
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
		}

		private async Task<(string, List<RvrObservation>)> FetchRvrObservationAsync(string airportFaaId, string url, CancellationToken stoppingToken)
		{
			using var stream = await _httpClient.GetStreamAsync(Constants.Urls.FaaRvrBase + url, stoppingToken);
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
					EdgeLightSetting = string.IsNullOrEmpty(tds[3].TextContent.Trim()) ? null : int.Parse(tds[3].TextContent.Trim()),
					CenterlineLightSetting = string.IsNullOrEmpty(tds[4].TextContent.Trim()) ? null : int.Parse(tds[4].TextContent.Trim())
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

		private static RvrTrend? ParseTrend(string text)
		{
			text = text.Trim();
			return text switch
			{
				string s when string.IsNullOrEmpty(s) => null,
				string s when s.Contains('▲')		  => RvrTrend.Increasing,
				string s when s.Contains('▼')		  => RvrTrend.Decreasing,
				_									  => RvrTrend.Steady
			};
		}

		[GeneratedRegex("[0-9]+")]
		private static partial Regex NumberRegex();
	}
}

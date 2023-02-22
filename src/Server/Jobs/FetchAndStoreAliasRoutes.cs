using Coravel.Invocable;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using ZoaIds.Server.Data;
using ZoaIds.Shared.Models;
using RouteAircraftType = ZoaIds.Shared.Models.RouteRule.RouteAircraftType;
using System.Text.RegularExpressions;
using AngleSharp.Dom;

namespace ZoaIds.Server.Jobs;

public partial class FetchAndStoreAliasRoutes : IInvocable
{
	private readonly ILogger<FetchAndStoreAliasRoutes> _logger;
	private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
	private readonly HttpClient _httpClient;

	public FetchAndStoreAliasRoutes(ILogger<FetchAndStoreAliasRoutes> logger, IDbContextFactory<ZoaIdsContext> contextFactory, HttpClient httpClient)
	{
		_logger = logger;
		_httpClient = httpClient;
		_contextFactory = contextFactory;
	}

	public async Task Invoke()
	{
		// TODO need to add some try catch exception handling?
		
		using var responseStream = await _httpClient.GetStreamAsync(Constants.Urls.ZoaAliasFile);
		using var reader = new StreamReader(responseStream);
		using var db = await _contextFactory.CreateDbContextAsync();
		_logger.LogInformation("Fetched ZOA Alias file from: {url}", Constants.Urls.ZoaAliasFile);

		// Read the first row and get the id string
		var firstLine = reader.ReadLine();

		// Query database to get the most recent successful fetch of the NASR data
		var lastUpdated = await db.CompletedJobs
			.AsNoTracking()
			.Where(j => j.Caller == GetType().Name && j.JobKey == "fetch" && j.ExitStatus == JobExitStatus.Success)
			.OrderByDescending(j => j.Time)
			.FirstOrDefaultAsync();

		// Return if there are no updates to CSV data
		if (firstLine == lastUpdated?.JobValue)
		{
			_logger.LogInformation("No updates to ZOA Alias file, exiting fetch task");
			return;
		}

		var rules = new List<RouteRule>();
		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			if (line is not null && TryParseRouteRule(line, out var routeRule))
			{
				rules.Add(routeRule);
			}
		}

		// Delete all existing rules and add all newly found rules
		var numDeleted = await db.AliasRouteRules.ExecuteDeleteAsync();
		_logger.LogInformation("Deleted {num} records from the Alias Rules table", numDeleted);
		await db.AliasRouteRules.AddRangeAsync(rules);
		await db.SaveChangesAsync();
		_logger.LogInformation("Added {num} records to Alias Rules table", rules.Count);

		// Save a record to the table of successful jobs
		var job = new ApplicationJob()
		{
			Caller = GetType().Name,
			Time = DateTime.UtcNow,
			JobKey = "fetch",
			JobValue = firstLine,
			ExitStatus = JobExitStatus.Success
		};
		await db.CompletedJobs.AddAsync(job);
		await db.SaveChangesAsync();
	}

	private static bool TryParseRouteRule(string line, out RouteRule? routeRule)
	{
		routeRule = null;

		// Return early if the given line is null or not a route command
		if (line is null || !AmRteRegex().IsMatch(line))
		{
			return false;
		}

		var commandMatch = CommandNameRegex().Match(line);
		var routeMatch = RouteRegex().Match(line);
		if (commandMatch.Success && routeMatch.Success)
		{
			routeRule = new RouteRule
			{
				DepartureAirport = commandMatch.Groups[1].Value.ToUpper(),
				DepartureRunway = string.IsNullOrEmpty(commandMatch.Groups[2].Value) ? null : int.Parse(commandMatch.Groups[2].Value),
				ArrivalAirport = commandMatch.Groups[3].Value.ToUpper(),
				ArrivalRunway = string.IsNullOrEmpty(commandMatch.Groups[4].Value) ? null : int.Parse(commandMatch.Groups[4].Value),
				AllowedAircraftType = commandMatch.Groups[5] is null
					? (RouteAircraftType.Jet | RouteAircraftType.Turboprop | RouteAircraftType.Prop)
					: RouteRule.StringToType(commandMatch.Groups[5].Value),
				Route = routeMatch.Groups[2].Value.Trim()
			};
			return true;
		}
		else
		{
			return false;
		}
	}

	[GeneratedRegex(@"\.am rte")]
	private static partial Regex AmRteRegex();

	[GeneratedRegex(@"([a-zA-Z0-9]{3})([0-9]{0,2})([a-zA-Z0-9]{3})([0-9]{0,2})([TPJtpj]?)")]
	private static partial Regex CommandNameRegex();

	[GeneratedRegex(@"\.am rte (\$route)?([^\$]*)(\$route)?")]
	private static partial Regex RouteRegex();
}
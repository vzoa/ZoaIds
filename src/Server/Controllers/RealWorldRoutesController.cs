using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoaIds.Server.Data;
using ZoaIds.Server.Services;
using ZoaIds.Shared.Models;

namespace ZoaIds.Server.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class RealWorldRoutesController : ControllerBase
{
	private readonly ILogger<RealWorldRoutesController> _logger;
	private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
	private readonly IRouteSummaryService _routeService;

	public RealWorldRoutesController(ILogger<RealWorldRoutesController> logger, IDbContextFactory<ZoaIdsContext> contextFactory, IRouteSummaryService routeService)
	{
		_logger = logger;
		_contextFactory = contextFactory;
		_routeService = routeService;
	}

	[HttpGet("summary")]
	public async Task<IActionResult> GetRouteSummary([FromQuery] string departure, [FromQuery] string arrival)
	{
		// Connect to db and see if an entry exists for this route pair
		using var db = await _contextFactory.CreateDbContextAsync();
		var existingRoute = await db.RealWorldRoutings
			.Where(r => r.DepartureIcaoId == departure.ToUpper() && r.ArrivalIcaoId == arrival.ToUpper())
			.FirstOrDefaultAsync();

		// If the route exists and it's newer than the TTL (default 20 min), return from DB
		if (existingRoute is not null && !IsStale(db, existingRoute))
		{
			return Ok(existingRoute);
		}
		else
		{
			// Route could exist in DB but be stale. If that's the case we need to delete it
			if (existingRoute is not null)
			{
				db.RealWorldRoutings.Remove(existingRoute);
			}

			// Fetch fresh route from FlightAware and store
			var newRoute = await _routeService.FetchRoutesAsync(departure.ToUpper(), arrival.ToUpper());
			await db.RealWorldRoutings.AddAsync(newRoute);
			await db.SaveChangesAsync();
			return Ok(newRoute);
		}
	}

	private static bool IsStale(ZoaIdsContext db, RealWorldRouting route)
	{
		var lastFetch = (DateTime)db.Entry(route).Property("Created").CurrentValue;
		return (DateTime.UtcNow - lastFetch).TotalSeconds > Constants.RoutesCacheTtlSeconds;
	}
}

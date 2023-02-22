using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoaIds.Server.Data;
using ZoaIds.Shared.Models;
using ZoaIds.Shared;

namespace ZoaIds.Server.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AliasRoutesController : ControllerBase
{
	private readonly ILogger<AliasRoutesController> _logger;
	private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

	public AliasRoutesController(ILogger<AliasRoutesController> logger, IDbContextFactory<ZoaIdsContext> contextFactory)
	{
		_logger = logger;
		_contextFactory = contextFactory;
	}

	[HttpGet]
	public async Task<IActionResult> GetAliasRouteRules([FromQuery] string departure, [FromQuery] string arrival)
	{
		departure = Helpers.SanitizeAirportIcaoToFaa(departure);
		arrival = Helpers.SanitizeAirportIcaoToFaa(arrival);
		
		// Connect to db and see if an entry exists for this route pair
		using var db = await _contextFactory.CreateDbContextAsync();
		var rules = await db.AliasRouteRules
			.Where(r => r.DepartureAirport == departure && r.ArrivalAirport == arrival)
			.ToListAsync();

		// Return any rules that are found, or an empty list if none
		return Ok(rules);
	}
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoaIds.Server.Data;
using ZoaIds.Shared;
using ZoaIds.Shared.Models;

namespace ZoaIds.Server.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AirportsController : ControllerBase
{
	private readonly ILogger<AirportsController> _logger;
	private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

	public AirportsController(ILogger<AirportsController> logger, IDbContextFactory<ZoaIdsContext> contextFactory)
	{
		_logger = logger;
		_contextFactory = contextFactory;
	}

	[HttpGet("{airportIds}")]
	public async Task<IActionResult> GetAirportInfo(string airportIds, [FromQuery] string idType = "icao")
	{
		// TODO -- need to add some error handling

		var airportIdArray = airportIds.Split(',').Select(id => id.ToUpper()).ToArray();

		using var db = await _contextFactory.CreateDbContextAsync();
		var returnAirports = idType.ToLower() switch
		{
			"faa"  => await db.Airports.Where(a => airportIdArray.Contains(a.FaaId)).ToListAsync(),
			"icao" => await db.Airports.Where(a => airportIdArray.Contains(a.IcaoId)).ToListAsync(),
			_      => new List<Airport>()
		};

		return returnAirports.Count > 0 ? Ok(returnAirports) : NotFound();
	}
}
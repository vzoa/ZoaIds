using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoaIds.Server.Data;
using ZoaIds.Shared.Models;

namespace ZoaIds.Server.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AirlinesController : ControllerBase
{
	private readonly ILogger<AirlinesController> _logger;
	private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

	public AirlinesController(ILogger<AirlinesController> logger, IDbContextFactory<ZoaIdsContext> contextFactory)
	{
		_logger = logger;
		_contextFactory = contextFactory;
	}

	[HttpGet("{search}")]
	public async Task<IActionResult> GetAirlineInfo(string search, [FromQuery] string queryType = "code")
	{
		using var db = await _contextFactory.CreateDbContextAsync();
		List<AirlineInfo>? returnList = null;

		returnList = queryType switch
		{
			"code"     => await db.Airlines.Where(t => t.IcaoId == search.ToUpper()).ToListAsync(),
			"callsign" => await db.Airlines.Where(t => t.Callsign != null && t.Callsign.ToUpper().Contains(search.ToUpper())).ToListAsync(),
			_          => null
		};

		return (returnList?.Count > 0) ? Ok(returnList) : NotFound();
	}
}
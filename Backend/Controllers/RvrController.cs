using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RvrController : ControllerBase
{
    private readonly ILogger<RvrController> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public RvrController(ILogger<RvrController> logger, IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    [HttpGet("{airportIds}")]
    public async Task<IActionResult> GetAirportRvrs(string airportIds)
    {
        // TODO -- need to add some error handling

        var airportIdArray = airportIds.Split(',').Select(id => Helpers.SanitizeAirportIcaoToFaa(id)).ToArray();

        using var db = await _contextFactory.CreateDbContextAsync();
        var returnRvrs = await db.RvrObservations.Where(r => airportIdArray.Contains(r.AirportFaaId)).ToListAsync();

        return returnRvrs.Count > 0 ? Ok(returnRvrs) : NotFound();
    }

    [HttpGet("{airportId}/{runwayIds}")]
    public async Task<IActionResult> GetRunwayRvrsForAirport(string airportId, string runwayIds)
    {
        // TODO -- need to add some error handling

        var runwayIdArray = runwayIds.Split(',').Select(id => id.ToUpper()).ToArray();

        using var db = await _contextFactory.CreateDbContextAsync();
        var returnRvrs = await db.RvrObservations
            .Where(r => r.AirportFaaId == Helpers.SanitizeAirportIcaoToFaa(airportId))
            .Where(r => runwayIdArray.Contains(r.RunwayEndName))
            .ToListAsync();

        return returnRvrs.Count > 0 ? Ok(returnRvrs) : NotFound();
    }
}
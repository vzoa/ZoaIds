using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Models;

namespace ZoaIdsBackend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AircraftTypesController : ControllerBase
{
    private readonly ILogger<AircraftTypesController> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public AircraftTypesController(ILogger<AircraftTypesController> logger, IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    [HttpGet("{search}")]
    public async Task<IActionResult> GetAircraftType(string search, [FromQuery] string queryType = "code")
    {
        using var db = await _contextFactory.CreateDbContextAsync();
        List<AircraftTypeInfo>? returnList = null;

        returnList = queryType switch
        {
            "code" => await db.AircraftTypes.Where(t => t.IcaoId == search.ToUpper()).ToListAsync(),
            "model" => await db.AircraftTypes.Where(t => t.Model.ToUpper().Contains(search.ToUpper())).ToListAsync(),
            _ => null
        };

        return returnList?.Count > 0 ? Ok(returnList) : NotFound();
    }
}



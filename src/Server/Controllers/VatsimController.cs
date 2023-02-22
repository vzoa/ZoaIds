using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using ZoaIds.Server.Data;
using System.Text.Json;
using ZoaIds.Shared.ExternalDataModels;

namespace ZoaIds.Server.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class VatsimController : ControllerBase
{
    private readonly ILogger<VatsimController> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public VatsimController(ILogger<VatsimController> logger, IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetLatestDatafeed()
    {
		// Get the latest snapshot
		using var db = await _contextFactory.CreateDbContextAsync();
        var snapshot = await GetLatestSnapshotFromDb(db);

        return (snapshot is not null) ? Content(snapshot.RawJson, MediaTypeNames.Application.Json) : StatusCode(StatusCodes.Status503ServiceUnavailable);
    }

    [HttpGet("{jsonSection}")]
    public async Task<IActionResult> GetSpecifiedSection(string jsonSection)
    {
        // Get the latest snapshot
        using var db = await _contextFactory.CreateDbContextAsync();
        var snapshot = await GetLatestSnapshotFromDb(db);

        // Return early if we can't find a snapsot
        if (snapshot is null)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        // Begin parsing snapshot
        using var jsonDoc = JsonDocument.Parse(snapshot.RawJson);
        var root = jsonDoc.RootElement;
        
        // Check if the JSON snapshot contains a section directly under the root corresponding
        // to the request. Return as JSON string if exists, otherwise 404
        if (root.TryGetProperty(jsonSection, out var element))
        {
            return Content(element.ToString(), MediaTypeNames.Application.Json);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("pilots/history/{callsign:regex(^[[a-zA-Z0-9]]+)}")]
    public async Task<IActionResult> GetPilotHistory(string callsign, [FromQuery] int? cid = null)
    {
        var returnPilotSnapshots = new List<VatsimJsonPilot>();
		using var db = await _contextFactory.CreateDbContextAsync();

        var snapshots = await db.VatsimSnapshots.AsNoTracking().OrderByDescending(x => x.Time).ToListAsync();

        foreach (var snapshot in snapshots)
        {
            var json = JsonSerializer.Deserialize<VatsimJsonRoot>(snapshot.RawJson);
            if (cid is null)
            {
                var foundPilot = json.Pilots.Where(p => p.Callsign.Equals(callsign, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (foundPilot is not null)
                {
                    cid = foundPilot.Cid;
                    returnPilotSnapshots.Add(foundPilot);
                }
            }
            else
            {
                var foundPilot = json.Pilots.Where(p => p.Callsign.Equals(callsign, StringComparison.OrdinalIgnoreCase) && p.Cid == cid).FirstOrDefault();
                if (foundPilot is not null)
                {
                    returnPilotSnapshots.Add(foundPilot);
                }
			}
        }

        return Ok(returnPilotSnapshots);
	}

    // Helper function to get the latest VATSIM datafeed snapshot from database
    private static Task<VatsimSnapshot?> GetLatestSnapshotFromDb(ZoaIdsContext db)
    {
        return db.VatsimSnapshots
            .AsNoTracking()
            .OrderByDescending(x => x.Time)
            .FirstOrDefaultAsync();
    }
}
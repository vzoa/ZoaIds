﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DatisController : ControllerBase
{
    private readonly ILogger<DatisController> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public DatisController(ILogger<DatisController> logger, IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDatis()
    {
        using var db = await _contextFactory.CreateDbContextAsync();
        return Ok(await db.Atises.ToDictionaryAsync(a => a.UniqueId!, a => a));
    }

    [HttpGet("{airportIds}")]
    public async Task<IActionResult> GetAirportDatis(string airportIds)
    {
        // TODO -- need to add some error handling

        var airportIdArray = airportIds.Split(',').Select(id => id.ToUpper()).ToArray();

        using var db = await _contextFactory.CreateDbContextAsync();
        var returnAtis = await db.Atises.Where(a => airportIdArray.Contains(a.IcaoId)).ToListAsync();

        return returnAtis.Count switch
        {
            1 => Ok(returnAtis.First()),
            > 1 => Ok(returnAtis.ToDictionary(a => a.UniqueId!, a => a)),
            _ => NotFound()
        };
    }
}
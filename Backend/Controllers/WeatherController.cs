using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly ILogger<WeatherController> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly HttpClient _httpClient;
    private static readonly string baseUrl = "https://api.aviationapi.com/v1/weather/";

    public WeatherController(ILogger<WeatherController> logger, IDbContextFactory<ZoaIdsContext> contextFactory, HttpClient httpClient)
    {
        _logger = logger;
        _contextFactory = contextFactory;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    [HttpGet("metar/{airportId}")]
    public async Task<IActionResult> GetMetar(string airportId)
    {
        // TODO -- need to add some error handling

        using var db = await _contextFactory.CreateDbContextAsync();
        var metar = await db.Metars.Where(m => m.StationId == airportId.ToUpper()).SingleOrDefaultAsync();
        if (metar is not null)
        {
            return Ok(metar);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("taf/{airportId}")]
    public async Task<IActionResult> GetTaf(string airportId)
    {
        // TODO -- need to add some error handling and maybe dedupe with above

        var result = await _httpClient.GetStringAsync($"taf?apt={airportId}");
        return Content(result, MediaTypeNames.Application.Json);
    }
}
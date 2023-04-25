using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ZoaIdsBackend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ChartsController : ControllerBase
{
    private readonly ILogger<ChartsController> _logger;
    private readonly HttpClient _httpClient;

    public ChartsController(ILogger<ChartsController> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(Constants.Urls.ChartsApiEndpoint);
    }

    [HttpGet("{airportId}")]
    public async Task<IActionResult> GetAirportCharts(string airportId)
    {
        // TODO -- need to add some error handling

        var result = await _httpClient.GetStringAsync($"?apt={airportId}"); // TODO maybe implement polly for resiliency
        return Content(result, MediaTypeNames.Application.Json);
    }
}
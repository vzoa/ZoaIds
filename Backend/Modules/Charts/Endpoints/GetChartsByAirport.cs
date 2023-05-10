using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Mime;

namespace ZoaIdsBackend.Modules.Charts.Endpoints;

public class AirportRequest
{
    public string Id { get; set; } = string.Empty;
}

public class AirportRequestValidator : Validator<AirportRequest>
{
    public AirportRequestValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .WithMessage("Airport ID required");
    }
}

public class GetChartsByAirport : Endpoint<AirportRequest, string>
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly IMemoryCache _cache;

    public GetChartsByAirport(HttpClient httpClient, IOptionsMonitor<AppSettings> appSettings, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _appSettings = appSettings;
        _httpClient.BaseAddress = new Uri(_appSettings.CurrentValue.Urls.ChartsApiEndpoint);
        _cache = cache;
    }

    public override void Configure()
    {
        //Get("/charts/{id}");
        Get("/charts/{id}", "/airports/{id}/charts");
        ResponseCache(_appSettings.CurrentValue.CacheTtls.Charts);
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(AirportRequest request, CancellationToken c)
    {
        if (_cache.TryGetValue<string>(MakeCacheKey(request.Id), out var result))
        {
            await SendStringAsync(result!, contentType: MediaTypeNames.Application.Json);
        }
        else
        {
            var fetchedResult = await _httpClient.GetStringAsync($"?apt={request.Id}", c); // TODO maybe implement polly for resiliency

            if (fetchedResult is null)
            {
                ThrowError("Effor fetching charts from AviationApi");
            }
            else
            {
                var expiration = DateTimeOffset.UtcNow.AddSeconds(_appSettings.CurrentValue.CacheTtls.Charts);
                _cache.Set(MakeCacheKey(request.Id), fetchedResult, expiration);
                await SendStringAsync(fetchedResult, contentType: MediaTypeNames.Application.Json);
            }
        }
    }

    private static string MakeCacheKey(string id) => $"ChartsCacheKey:{id.ToUpper()}";
}

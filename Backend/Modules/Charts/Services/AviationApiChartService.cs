using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ZoaIdsBackend.Modules.Charts.Models;

namespace ZoaIdsBackend.Modules.Charts.Services;

public class AviationApiChartService
{
    private readonly ILogger<AviationApiChartService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly IMemoryCache _cache;

    public AviationApiChartService(ILogger<AviationApiChartService> logger, HttpClient httpClient, IOptionsMonitor<AppSettings> appSettings, IMemoryCache cache)
    {
        _logger = logger;
        _httpClient = httpClient;
        _appSettings = appSettings;
        _httpClient.BaseAddress = new Uri(_appSettings.CurrentValue.Urls.ChartsApiEndpoint);
        _cache = cache;
    }

    public async Task<ICollection<Chart>> GetChartsForId(string id, CancellationToken c = default)
    {
        if (_cache.TryGetValue<ICollection<Chart>>(MakeCacheKey(id), out var result))
        {
            return result ?? new List<Chart>();
        }

        // Get Charts data from API and change to our own format
        var chartsDict = new Dictionary<string, Chart>();
        foreach (var chart in await GetChartsDtoForId(id, c))
        {
            if (IsContinuationPage(chart, out var name, out var page))
            {
                if (name is not null && chartsDict.TryGetValue(name, out var existingChart))
                {
                    var newPage = new ChartPage
                    {
                        PageNumber = page ?? 1,
                        PdfName = chart.PdfName,
                        PdfPath = chart.PdfPath
                    };
                    existingChart.Pages.Add(newPage);
                }
                else if (name is not null)
                {
                    chartsDict[name] = MakeChart(chart, name, page ?? 1);
                }
            }
            else
            {
                chartsDict[chart.ChartName] = MakeChart(chart);
            }
        }

        // Cache and return
        var expiration = DateTimeOffset.UtcNow.AddSeconds(_appSettings.CurrentValue.CacheTtls.Charts);
        _cache.Set<ICollection<Chart>>(MakeCacheKey(id), chartsDict.Values, expiration);
        return chartsDict.Values;
    }
    
    private async Task<IEnumerable<AviationApiChartDto>> GetChartsDtoForId(string id, CancellationToken c = default)
    {
        var apiJson = await _httpClient.GetFromJsonAsync<Dictionary<string, ICollection<AviationApiChartDto>>>($"?apt={id}", c);
        return apiJson is not null ? apiJson.Values.SelectMany(c => c) : Enumerable.Empty<AviationApiChartDto>();
    }

    private static bool IsContinuationPage(AviationApiChartDto chartDto, out string? name, out int? page)
    {
        name = null; 
        page = null;
        
        if (chartDto.ChartName.Contains(", CONT."))
        {
            var split = chartDto.ChartName.Split(", CONT.");
            name = split[0];
            page = int.Parse(split[1]) + 1; // Means that "CONT.1" returns page 2
            return true;
        }
        else
        {
            return false;
        }
    }

    private static Chart MakeChart(AviationApiChartDto chartDto, string name = "", int pageNumber = -1)
    {
        return new Chart
        {
            AirportName = chartDto.AirportName,
            FaaIdent = chartDto.FaaIdent,
            IcaoIdent = chartDto.IcaoIdent,
            ChartSeq = chartDto.ChartSeq,
            ChartCode = chartDto.ChartCode,
            ChartName = string.IsNullOrEmpty(name) ? chartDto.ChartName : name,
            Pages = new List<ChartPage>
            {
                new ChartPage
                {
                    PageNumber = pageNumber == -1 ? 1 : pageNumber,
                    PdfName = chartDto.PdfName,
                    PdfPath = chartDto.PdfPath
                }
            }
        };
    }

    private static string MakeCacheKey(string id) => $"ChartsCacheKey:{id.ToUpper()}";
}

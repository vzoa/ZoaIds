using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Options;
using ZoaIdsBackend.Modules.Charts.Models;
using ZoaIdsBackend.Modules.Charts.Services;

namespace ZoaIdsBackend.Modules.Charts.Endpoints;

public class AirportRequest
{
    public string Id { get; set; } = string.Empty;
}

public class AllChartsResponse
{
    public string AirportName { get; set; } = string.Empty;
    public string FaaIdent { get; set; } = string.Empty;
    public string IcaoIdent { get; set; } = string.Empty;
    public IEnumerable<SingleChartResponse > Charts { get; set; } = Enumerable.Empty<SingleChartResponse>();

}

public class SingleChartResponse
{
    public string ChartSeq { get; set; } = string.Empty;
    public string ChartCode { get; set; } = string.Empty;
    public string ChartName { get; set; } = string.Empty;
    public IEnumerable<ChartPage> Pages { get; set; } = Enumerable.Empty<ChartPage>();
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

public class GetChartsByAirport : Endpoint<AirportRequest, AllChartsResponse>
{
    private readonly IOptionsMonitor<AppSettings> _appSettings;
    private readonly AviationApiChartService _chartService;

    public GetChartsByAirport(IOptionsMonitor<AppSettings> appSettings, AviationApiChartService chartService)
    {
        _appSettings = appSettings;
        _chartService = chartService;
    }

    public override void Configure()
    {
        //Get("/charts/{id}");
        Get(ChartsModule.BaseUri + "/{id}", "/airports/{id}/charts");
        ResponseCache(_appSettings.CurrentValue.CacheTtls.Charts);
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(AirportRequest request, CancellationToken c)
    {
        var charts = await _chartService.GetChartsForId(request.Id, c);
        if (!charts.Any())
        {
            await SendNotFoundAsync();
        }
        
        var response = new AllChartsResponse
        {
            AirportName = charts.First().AirportName,
            FaaIdent = charts.First().FaaIdent,
            IcaoIdent = charts.First().IcaoIdent,
            Charts = charts.Select(c => new SingleChartResponse
            {
                ChartSeq = c.ChartSeq,
                ChartCode = c.ChartCode,
                ChartName = c.ChartName,
                Pages = c.Pages
            })
        };

        await SendAsync(response);
    }
}

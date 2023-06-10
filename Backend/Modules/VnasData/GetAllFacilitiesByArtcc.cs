using FastEndpoints;
using ZoaIdsBackend.Modules.VnasData.Models;
using ZoaIdsBackend.Modules.VnasData.Services;

namespace ZoaIdsBackend.Modules.VnasData.Endpoints;

public class VnasDataRequest
{
    public string ArtccId { get; set; } = string.Empty;
}

public class FacilityResponse
{
    public string Id { get; set; } = string.Empty;
    public Facility.FacilityType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public IEnumerable<PositionResponse> Positions { get; set; } = new List<PositionResponse>();
}

public class PositionResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string RadioName { get; set; } = string.Empty;
    public string Callsign { get; set; } = string.Empty;
    public int Frequency { get; set; } = 0;
}

public class GetAirportSituation : Endpoint<VnasDataRequest, IEnumerable<FacilityResponse>>
{
    private readonly CachedVnasDataService _vnasDataService;

    public GetAirportSituation(CachedVnasDataService vnasDataService)
    {
        _vnasDataService = vnasDataService;
    }

    public override void Configure()
    {
        Get("/artccs/{@id}/facilities", x => new { x.ArtccId });
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(VnasDataRequest request, CancellationToken c)
    {
        var facilities = await _vnasDataService.GetArtccFacilities(request.ArtccId, c);
        if (facilities is null)
        {
            await SendNotFoundAsync();
        }
        else
        {
            await SendAsync(facilities.Select(f => MapToResponse(f)));
        }
    }

    private static FacilityResponse MapToResponse(Facility facility)
    {
        return new FacilityResponse
        {
            Id = facility.Id,
            Type = facility.Type,
            Name = facility.Name,
            Positions = facility.Positions.Select(p => new PositionResponse
            {
                Id = p.Id,
                Name = p.Name,
                RadioName = p.RadioName,
                Callsign = p.Callsign,
                Frequency = p.Frequency,
            })
        };
    }
}

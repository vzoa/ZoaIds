using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Modules.AircraftTypes.Endpoints;

public class SingleAircraftRequest
{
    public string IcaoTypeDesignator { get; set; } = string.Empty;
}

public class SingleAircraftResponse
{
    public string TypeDesignator { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string EngineType { get; set; } = string.Empty;
    public string EngineCount { get; set; } = string.Empty;
    public string IcaoWakeTurbulenceCategory { get; set; } = string.Empty;
    public string FaaWeightClass { get; set; } = string.Empty;
    public string ConslidatedWakeTurbulenceCategory { get; set; } = string.Empty;
    public string SameRunwaySeparationCategory { get; set; } = string.Empty;
    public string LandAndHoldShortGroup { get; set; } = string.Empty;
    public ICollection<AircraftModelResponse> Models { get; set; } = new List<AircraftModelResponse>();
}

public class AircraftModelResponse
{
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;

    public AircraftModelResponse(string manufactuer, string model)
    {
        Manufacturer = manufactuer;
        Model = model;
    }
}

public class GetAircraftInfoByType : Endpoint<SingleAircraftRequest, SingleAircraftResponse>
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public GetAircraftInfoByType(IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public override void Configure()
    {
        Get("/aircraft/{@type}", x => new { x.IcaoTypeDesignator });
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(SingleAircraftRequest request, CancellationToken c)
    {
        using var db = await _contextFactory.CreateDbContextAsync(c);
        var response = await MakeAircraftResponseAsync(request.IcaoTypeDesignator, db, c);
        if (response is null)
        {
            await SendNotFoundAsync();
        }
        else
        {
            await SendAsync(response);
        }
    }

    internal static async Task<SingleAircraftResponse?> MakeAircraftResponseAsync(string id, ZoaIdsContext db, CancellationToken c = default)
    {
        var aircraft = db.AircraftTypes.Where(a => a.IcaoId == id.ToUpper());
        var first = await aircraft.FirstOrDefaultAsync(c);
        if (first is null)
        {
            return null;
        }
        else
        {
            var aircraftList = await aircraft.ToListAsync(c);
            var response = new SingleAircraftResponse
            {
                TypeDesignator = first.IcaoId,
                Class = first.Class,
                EngineType = first.EngineType,
                EngineCount = first.EngineCount,
                IcaoWakeTurbulenceCategory = first.IcaoWakeTurbulenceCategory,
                FaaWeightClass = first.FaaWeightClass,
                ConslidatedWakeTurbulenceCategory = first.ConslidatedWakeTurbulenceCategory,
                SameRunwaySeparationCategory = first.SameRunwaySeparationCategory,
                LandAndHoldShortGroup = first.LandAndHoldShortGroup,
                Models = aircraftList.Select(a => new AircraftModelResponse(a.Manufacturer, a.Model)).ToList(),
            };
            return response;
        }
    }
}

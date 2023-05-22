using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.VatsimData.Models;
using ZoaIdsBackend.Modules.VatsimData.Repositories;

namespace ZoaIdsBackend.Modules.VatsimData.Endpoints;

public class AirportSituationRequest
{
    public string FaaId { get; set; } = string.Empty;
}

public class AirportSituationResponse
{
    public string FaaId { get; set; } = string.Empty;
    //public ICollection<VatsimJsonPilot> OnGround { get; set; } = new List<VatsimJsonPilot>();
    public OnGroundResponse OnGround { get; set; } = new();
    public ICollection<VatsimJsonPilot> EnrouteArrivals { get; set; } = new List<VatsimJsonPilot>();
    public ICollection<VatsimJsonPilot> EnrouteDepartures { get; set; } = new List<VatsimJsonPilot>();
    public ICollection<VatsimJsonController> Controllers { get; set; } = new List<VatsimJsonController>();
    public ICollection<VatsimJsonAtis> Atis { get; set; } = new List<VatsimJsonAtis>();

    public class OnGroundResponse
    {
        public ICollection<VatsimJsonPilot> Departures { get; set; } = new List<VatsimJsonPilot>();
        public ICollection<VatsimJsonPilot> Arrivals { get; set; } = new List<VatsimJsonPilot>();
        public ICollection<VatsimJsonPilot> NoFlightPlan { get; set; } = new List<VatsimJsonPilot>();
    }
}

public class AirportSituationValidator : Validator<AirportSituationRequest>
{
    public AirportSituationValidator()
    {
        RuleFor(r => r.FaaId)
            .NotEmpty()
            .WithMessage("Airport FAA ID required");
    }
}

public class GetAirportSituation : Endpoint<AirportSituationRequest, AirportSituationResponse>
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
    private readonly IVatsimDataRepository _repository;
    private readonly IMemoryCache _cache;

    public GetAirportSituation(IDbContextFactory<ZoaIdsContext> contextFactory, IVatsimDataRepository repository, IMemoryCache cache)
    {
        _contextFactory = contextFactory;
        _repository = repository;
        _cache = cache;
    }

    public override void Configure()
    {
        Get(VatsimDataModule.BaseUri + "/airports/{@id}", x => new { x.FaaId });
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(AirportSituationRequest request, CancellationToken c)
    {
        var vatsimData = await _repository.GetLatestDataAsync(c);

        // Check if we have a cached result from the current VATSIM datafeed timestamp. If so return early
        if (_cache.TryGetValue<(string timestamp, AirportSituationResponse response)>(MakeCacheKey(request.FaaId.ToUpper()), out var cachedResult))
        {
            if (vatsimData is not null && vatsimData.General.Update == cachedResult.timestamp)
            {
                await SendAsync(cachedResult.response);
                return;
            }
        }

        // If not, make the new response
        using var db = await _contextFactory.CreateDbContextAsync(c);
        var id = request.FaaId.ToUpper();
        var airport = await db.Airports.FindAsync(id);
        if (airport is null)
        {
            ThrowError("Not a valid FAA LID");
        }
                
        var response = new AirportSituationResponse
        {
            FaaId = id,
            Atis = vatsimData.GetAtis(airport.IcaoId).ToList(),
            Controllers = vatsimData.GetCabControllersByPrefix(id).ToList(),
        };

        foreach (var pilot in vatsimData.Pilots)
        {
            if (pilot.FlightPlan is not null)
            {
                if (pilot.FlightPlan.Departure.Equals(airport.IcaoId, StringComparison.OrdinalIgnoreCase))
                {
                    if (pilot.IsOnGroundAtAirport(airport))
                    {
                        response.OnGround.Departures.Add(pilot);
                    }
                    else
                    {
                        response.EnrouteDepartures.Add(pilot);
                    }
                }
                else if (pilot.FlightPlan.Arrival.Equals(airport.IcaoId, StringComparison.OrdinalIgnoreCase))
                {
                    if (pilot.IsOnGroundAtAirport(airport))
                    {
                        response.OnGround.Arrivals.Add(pilot);
                    }
                    else
                    {
                        response.EnrouteArrivals.Add(pilot);
                    }
                }
            }
            else if (pilot.IsOnGroundAtAirport(airport))
            {
                response.OnGround.NoFlightPlan.Add(pilot);
            }
        }

        // Cache new result and return
        _cache.Set<(string, AirportSituationResponse)>(MakeCacheKey(request.FaaId.ToUpper()), (vatsimData.General.Update, response));
        await SendAsync(response);
    }

    private static string MakeCacheKey(string airportId) => $"AirportSituation:{airportId}";
}

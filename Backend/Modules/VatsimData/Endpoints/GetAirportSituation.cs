using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
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

    public GetAirportSituation(IDbContextFactory<ZoaIdsContext> contextFactory, IVatsimDataRepository repository)
    {
        _contextFactory = contextFactory;
        _repository = repository;
    }

    public override void Configure()
    {
        Get(VatsimDataModule.BaseUri + "/airports/{@id}", x => new { x.FaaId });
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(AirportSituationRequest request, CancellationToken c)
    {
        using var db = await _contextFactory.CreateDbContextAsync(c);

        // Find requested airport info
        var id = request.FaaId.ToUpper();
        var airport = await db.Airports.FindAsync(id);
        if (airport is null)
        {
            ThrowError("Not a valid FAA LID");
        }

        var root = await _repository.GetLatestDataAsync(c);
        var response = new AirportSituationResponse
        {
            FaaId = id,
            Atis = root.GetAtis(airport.IcaoId).ToList(),
            Controllers = root.GetCabControllersByPrefix(id).ToList(),
        };

        foreach (var pilot in root.Pilots)
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

        await SendAsync(response);
    }
}

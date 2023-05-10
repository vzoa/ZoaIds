using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.NasrData.Models;

namespace ZoaIdsBackend.Modules.NasrData.Endpoints;

public class MultipleAirportsRequest
{
    public ICollection<string> Faa { get; set; } = Array.Empty<string>();
    public ICollection<string> Icao { get; set; } = Array.Empty<string>();
}

public class AirportsRequestValidator : Validator<MultipleAirportsRequest>
{
    public AirportsRequestValidator()
    {
        RuleFor(r => r.Faa).NotEmpty().When(r => !r.Icao.Any()).WithMessage("Must request at least one airport");
        RuleFor(r => r.Icao).NotEmpty().When(r => !r.Faa.Any()).WithMessage("Must request at least one airport");
    }
}

public class GetAirportsById : Endpoint<MultipleAirportsRequest, ICollection<Airport>>
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public GetAirportsById(IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public override void Configure()
    {
        Get("/airports");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(MultipleAirportsRequest request, CancellationToken c)
    {
        var faaUppercase = request.Faa.Select(x => x.ToUpperInvariant());
        var icaoUppercase = request.Icao.Select(x => x.ToUpperInvariant());

        using var db = await _contextFactory.CreateDbContextAsync(c);
        var faaAirports = db.Airports.AsNoTracking().Where(a => faaUppercase.Contains(a.FaaId));
        var icaoAirports = db.Airports.AsNoTracking().Where(a => icaoUppercase.Contains(a.IcaoId));

        var returnAirports = Enumerable.Concat(faaAirports, icaoAirports).ToList();

        if (returnAirports.Count > 0)
        {
            await SendAsync(returnAirports);
        }
        else
        {
            await SendNotFoundAsync();
        }
    }
}

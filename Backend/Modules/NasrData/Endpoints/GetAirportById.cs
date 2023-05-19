using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ZoaIdsBackend.Data;
using ZoaIdsBackend.Modules.NasrData.Models;

namespace ZoaIdsBackend.Modules.NasrData.Endpoints;

public class SingleAirportRequest
{
    public string Id { get; set; } = string.Empty;
    public string IdType { get; set; } = "icao";
}

public class AirportRequestValidator : Validator<SingleAirportRequest>
{
    public static readonly string[] AcceptableTypes = new string[] { "icao", "faa" };

    public AirportRequestValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .WithMessage("Airport ID required");

        RuleFor(r => r.IdType)
            .Must(t => AcceptableTypes.Contains(t, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Must be one of: {string.Join(", ", AcceptableTypes)}");
    }
}

public class GetAirportById : Endpoint<SingleAirportRequest, Airport>
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public GetAirportById(IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public override void Configure()
    {
        Get("/airports/{id}");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(SingleAirportRequest request, CancellationToken c)
    {
        Expression<Func<Airport, bool>> predicate = request.IdType switch
        {
            "icao" => a => a.IcaoId == request.Id.ToUpperInvariant(),
            "faa" => a => a.FaaId == request.Id.ToUpperInvariant(),
            _ => throw new NotImplementedException()
        };

        using var db = await _contextFactory.CreateDbContextAsync(c);
        var returnAirport = await db.Airports.AsNoTracking().Where(predicate).SingleOrDefaultAsync(c);

        if (returnAirport is not null)
        {
            await SendAsync(returnAirport);
        }
        else
        {
            await SendNotFoundAsync();
        }
    }
}

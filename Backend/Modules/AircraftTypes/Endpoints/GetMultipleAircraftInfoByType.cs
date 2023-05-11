using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Modules.AircraftTypes.Endpoints;

public class MultipleAircraftRequest
{
    public ICollection<string> Id { get; set; } = new List<string>();
}

public class MultipleAircraftRequestValidator : Validator<MultipleAircraftRequest>
{
    public MultipleAircraftRequestValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty()
            .WithMessage("At least one Type Designator required");
    }
}

public class GetMultipleAircraftInfoByType : Endpoint<MultipleAircraftRequest, IEnumerable<SingleAircraftResponse>>
{
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public GetMultipleAircraftInfoByType(IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public override void Configure()
    {
        Get("/aircraft");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(MultipleAircraftRequest request, CancellationToken c)
    {
        using var db = await _contextFactory.CreateDbContextAsync(c);
        var responses = await Task.WhenAll(request.Id.Select(s => GetAircraftInfoByType.MakeAircraftResponseAsync(s, db, c)));
        var filteredResponses = responses.Where(r => r is not null).Select(r => r!);
        await SendAsync(filteredResponses);
    }
}

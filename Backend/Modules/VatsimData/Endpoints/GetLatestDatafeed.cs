using FastEndpoints;
using System.Net.Mime;
using ZoaIdsBackend.Modules.VatsimData.Models;
using ZoaIdsBackend.Modules.VatsimData.Repositories;

namespace ZoaIdsBackend.Modules.VatsimData.Endpoints;

public class GetLatestDatafeed : EndpointWithoutRequest<VatsimJsonRoot>
{
    private readonly IVatsimDataRepository _repository;

    public GetLatestDatafeed(IVatsimDataRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get(VatsimDataModule.BaseUri + "/datafeed");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        // Get latest snapshot from DB
        var snapshot = await _repository.GetLatestSnapshotAsync(c);

        // Return 404 if no snapshots found, or raw string of the JSON if found.
        // No need to serialize then deserialize immediately
        if (snapshot is null)
        {
            await SendNotFoundAsync();
        }
        else
        {
            await SendStringAsync(snapshot.RawJson, contentType: MediaTypeNames.Application.Json);
        }
    }
}

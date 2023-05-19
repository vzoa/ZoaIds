using FastEndpoints;
using System.Net.Mime;
using System.Text.Json;
using ZoaIdsBackend.Modules.VatsimData.Models;
using ZoaIdsBackend.Modules.VatsimData.Repositories;

namespace ZoaIdsBackend.Modules.VatsimData.Endpoints;

public class SectionRequest
{
    public string SectionName { get; set; } = string.Empty;
}

public class GetLatestDatafeedSection : Endpoint<SectionRequest, VatsimJsonRoot>
{
    private readonly IVatsimDataRepository _repository;

    public GetLatestDatafeedSection(IVatsimDataRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get(VatsimDataModule.BaseUri + "/datafeed/{@sectionName}", x => new { x.SectionName });
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(SectionRequest sectionRequest, CancellationToken c)
    {
        // Get latest snapshot from DB
        var snapshot = await _repository.GetLatestSnapshotAsync(c);

        // Return early if we can't find a snapsot
        if (snapshot is null)
        {
            await SendNotFoundAsync();
        }

        // Check if the JSON snapshot contains a section directly under the root corresponding
        // to the request. Return as JSON string if exists, otherwise 404
        using var jsonDoc = JsonDocument.Parse(snapshot.RawJson);
        var root = jsonDoc.RootElement;
        if (root.TryGetProperty(sectionRequest.SectionName, out var element))
        {
            await SendStringAsync(element.ToString(), contentType: MediaTypeNames.Application.Json);
        }
        else
        {
            ThrowError(r => r.SectionName, "Requested section does not exist in VATSIM JSON specification");
        }
    }
}

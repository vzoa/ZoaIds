using FastEndpoints;
using System.Text.Json;
using ZoaIdsBackend.Modules.VatsimData.Models;
using ZoaIdsBackend.Modules.VatsimData.Repositories;

namespace ZoaIdsBackend.Modules.VatsimData.Endpoints;

public class PilotRequest
{
    public string Callsign { get; set; }
    public int? Cid { get; set; } = null;
}

public class GetPilotTrack : Endpoint<PilotRequest, List<VatsimJsonPilot>>
{
    private readonly IVatsimDataRepository _repository;

    public GetPilotTrack(IVatsimDataRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get(VatsimDataModule.BaseUri + "/pilots/{callsign}/tracks");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(PilotRequest pilotRequest, CancellationToken c)
    {
        var returnPilotTracks = new List<VatsimJsonPilot>();
        var snapshots = await _repository.GetAllSnapshotsAsync(c);

        var searchForCid = pilotRequest.Cid;
        foreach (var snapshot in snapshots)
        {
            var json = JsonSerializer.Deserialize<VatsimJsonRoot>(snapshot.RawJson);

            var foundPilot = json.Pilots
                .Where(p => p.Callsign.Equals(pilotRequest.Callsign, StringComparison.OrdinalIgnoreCase) && (searchForCid is null || p.Cid == searchForCid)).FirstOrDefault();

            if (foundPilot is not null)
            {
                searchForCid ??= foundPilot.Cid;
                returnPilotTracks.Add(foundPilot);
            }
        }

        if (returnPilotTracks.Count > 0)
        {
            await SendAsync(returnPilotTracks);
        }
        else
        {
            await SendNotFoundAsync();
        }
    }
}
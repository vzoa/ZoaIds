using FastEndpoints;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZoaIdsBackend.Modules.VatsimData.Models;
using ZoaIdsBackend.Modules.VatsimData.Repositories;

namespace ZoaIdsBackend.Modules.VatsimData.Endpoints;

public class VatsimUserRequest
{
    public int Cid { get; set; } = 0;
}

public class VatsimUserResponse
{ 
    public VatsimUserDetailsResponse? UserDetails { get; set; }
    public VatsimUserStats? Stats { get; set; }
}

public class VatsimUserDetailsResponse
{
    public int Id { get; set; }

    public Rating Rating { get; set; }

    public Rating PilotRating { get; set; }

    public Rating MilitaryRating { get; set; }

    public DateTime? SuspensionDate { get; set; }

    [JsonPropertyName("reg_date")]
    public DateTime RegistrationDate { get; set; }

    [JsonPropertyName("region_id")]
    public string RegionId { get; set; }

    [JsonPropertyName("division_id")]
    public string DivisionId { get; set; }

    [JsonPropertyName("subdivision_id")]
    public string? SubdivisionId { get; set; }

    [JsonPropertyName("lastratingchange")]
    public DateTime? LastRatingChange { get; set; }
}

public class Rating
{
    public int Id { get; set; }
    public string ShortName { get; set; } = string.Empty;
    public string LongName { get; set; } = string.Empty;

    public Rating(int id, string shortName, string longName)
    {
        Id = id;
        ShortName = shortName;
        LongName = longName;
    }
}

public class GetVatsimUser : Endpoint<VatsimUserRequest, VatsimUserResponse>
{

    private readonly IVatsimDataRepository _repository;
    private readonly IHttpClientFactory _httpClientFactory;

    public GetVatsimUser(IVatsimDataRepository repository, IHttpClientFactory httpClientFactory)
    {
        _repository = repository;
        _httpClientFactory = httpClientFactory;
    }

    public override void Configure()
    {
        Get(VatsimDataModule.BaseUri + "/users/{@id}", x => new { x.Cid });
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(VatsimUserRequest request, CancellationToken c)
    {
        // Get latest snapshot from DB
        var snapshot = await _repository.GetLatestSnapshotAsync(c);
        var deserializedSnapshot = JsonSerializer.Deserialize<VatsimJsonRoot>(snapshot.RawJson);

        var httpClient = _httpClientFactory.CreateClient();
        var detailsTask = httpClient.GetFromJsonAsync<VatsimUserDetails>($"https://api.vatsim.net/v2/members/{request.Cid}");
        var statsTask = httpClient.GetAsync($"https://api.vatsim.net/v2/members/{request.Cid}/stats");

        await Task.WhenAll(detailsTask, statsTask);

        VatsimUserStats? stats = null;
        if (statsTask.Result.IsSuccessStatusCode)
        {
            stats = await statsTask.Result.Content.ReadFromJsonAsync<VatsimUserStats>(cancellationToken: c);
        }

        if (detailsTask.Result is null || deserializedSnapshot is null)
        {
            await SendNotFoundAsync();
            return;
        }

        var response = new VatsimUserResponse
        {
            UserDetails = MapVatsimUserDetailsResponse(detailsTask.Result, deserializedSnapshot),
            Stats = stats
        };

        await SendAsync(response);
    }

    private static VatsimUserDetailsResponse MapVatsimUserDetailsResponse(VatsimUserDetails details, VatsimJsonRoot snapshot)
    {
        var rating = snapshot.Ratings.Find(r => r.Id == details.Rating);
        var pilotRating = snapshot.PilotRatings.Find(r => r.Id == details.PilotRating);
        if (pilotRating is null)
        {
            pilotRating = new VatsimJsonPilotRating { Id = rating.Id, ShortName = rating.Short, LongName = rating.Long };
        }
        var militaryRating = snapshot.MilitaryRatings.Find(r => r.Id == details.MilitaryRating);

        return new VatsimUserDetailsResponse
        {
            Id = details.Id,
            Rating = new Rating(details.Rating, rating.Short, rating.Long),
            PilotRating = new Rating(details.Rating, pilotRating.ShortName, pilotRating.LongName),
            MilitaryRating = new Rating(details.Rating, militaryRating.ShortName, militaryRating.LongName),
            SuspensionDate = details.SuspensionDate,
            RegistrationDate = details.RegistrationDate,
            RegionId = details.RegionId,
            DivisionId = details.DivisionId,
            SubdivisionId = details.SubdivisionId,
            LastRatingChange = details.LastRatingChange
        };
    }
}
    
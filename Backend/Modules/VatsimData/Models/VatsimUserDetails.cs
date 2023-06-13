using System.Text.Json.Serialization;

namespace ZoaIdsBackend.Modules.VatsimData.Models;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class VatsimUserDetails
{
    public int Id { get; set; }

    public int Rating { get; set; }

    [JsonPropertyName("pilotrating")]
    public int PilotRating { get; set; }

    [JsonPropertyName("militaryrating")]
    public int MilitaryRating { get; set; }

    [JsonPropertyName("susp_date")]
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


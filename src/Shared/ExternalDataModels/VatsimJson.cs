using System.Text.Json.Serialization;

namespace ZoaIds.Shared.ExternalDataModels;

// Root myDeserializedClass = JsonSerializer.Deserialize<VatsimJsonRoot>(myJsonResponse);
// Generated from https://json2csharp.com/

public class VatsimJsonAtis
{
    [JsonPropertyName("cid")]
    public int? Cid { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("callsign")]
    public string Callsign { get; set; }

    [JsonPropertyName("frequency")]
    public string Frequency { get; set; }

    [JsonPropertyName("facility")]
    public int? Facility { get; set; }

    [JsonPropertyName("rating")]
    public int? Rating { get; set; }

    [JsonPropertyName("server")]
    public string Server { get; set; }

    [JsonPropertyName("visual_range")]
    public int? VisualRange { get; set; }

    [JsonPropertyName("atis_code")]
    public string AtisCode { get; set; }

    [JsonPropertyName("text_atis")]
    public List<string> TextAtis { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime? LastUpdated { get; set; }

    [JsonPropertyName("logon_time")]
    public DateTime? LogonTime { get; set; }
}

public class VatsimJsonController
{
    [JsonPropertyName("cid")]
    public int? Cid { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("callsign")]
    public string Callsign { get; set; }

    [JsonPropertyName("frequency")]
    public string Frequency { get; set; }

    [JsonPropertyName("facility")]
    public int? Facility { get; set; }

    [JsonPropertyName("rating")]
    public int? Rating { get; set; }

    [JsonPropertyName("server")]
    public string Server { get; set; }

    [JsonPropertyName("visual_range")]
    public int? VisualRange { get; set; }

    [JsonPropertyName("text_atis")]
    public List<string> TextAtis { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime? LastUpdated { get; set; }

    [JsonPropertyName("logon_time")]
    public DateTime? LogonTime { get; set; }
}

public class VatsimJsonFacility
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("short")]
    public string Short { get; set; }

    [JsonPropertyName("long")]
    public string Long { get; set; }
}

public class VatsimJsonFlightPlan
{
    [JsonPropertyName("flight_rules")]
    public string FlightRules { get; set; }

    [JsonPropertyName("aircraft")]
    public string Aircraft { get; set; }

    [JsonPropertyName("aircraft_faa")]
    public string AircraftFaa { get; set; }

    [JsonPropertyName("aircraft_short")]
    public string AircraftShort { get; set; }

    [JsonPropertyName("departure")]
    public string Departure { get; set; }

    [JsonPropertyName("arrival")]
    public string Arrival { get; set; }

    [JsonPropertyName("alternate")]
    public string Alternate { get; set; }

    [JsonPropertyName("cruise_tas")]
    public string CruiseTas { get; set; }

    [JsonPropertyName("altitude")]
    public string Altitude { get; set; }

    [JsonPropertyName("deptime")]
    public string Deptime { get; set; }

    [JsonPropertyName("enroute_time")]
    public string EnrouteTime { get; set; }

    [JsonPropertyName("fuel_time")]
    public string FuelTime { get; set; }

    [JsonPropertyName("remarks")]
    public string Remarks { get; set; }

    [JsonPropertyName("route")]
    public string Route { get; set; }

    [JsonPropertyName("revision_id")]
    public int? RevisionId { get; set; }

    [JsonPropertyName("assigned_transponder")]
    public string AssignedTransponder { get; set; }
}

public class VatsimJsonGeneralInfo
{
    [JsonPropertyName("version")]
    public int? Version { get; set; }

    [JsonPropertyName("reload")]
    public int? Reload { get; set; }

    [JsonPropertyName("update")]
    public string Update { get; set; }

    [JsonPropertyName("update_timestamp")]
    public DateTime? UpdateTimestamp { get; set; }

    [JsonPropertyName("connected_clients")]
    public int? ConnectedClients { get; set; }

    [JsonPropertyName("unique_users")]
    public int? UniqueUsers { get; set; }
}

public class VatsimJsonPilot
{
    [JsonPropertyName("cid")]
    public int? Cid { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("callsign")]
    public string Callsign { get; set; }

    [JsonPropertyName("server")]
    public string Server { get; set; }

    [JsonPropertyName("pilot_rating")]
    public int? PilotRating { get; set; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    [JsonPropertyName("altitude")]
    public int? Altitude { get; set; }

    [JsonPropertyName("groundspeed")]
    public int? Groundspeed { get; set; }

    [JsonPropertyName("transponder")]
    public string Transponder { get; set; }

    [JsonPropertyName("heading")]
    public int? Heading { get; set; }

    [JsonPropertyName("qnh_i_hg")]
    public double? QnhIHg { get; set; }

    [JsonPropertyName("qnh_mb")]
    public int? QnhMb { get; set; }

    [JsonPropertyName("flight_plan")]
    public VatsimJsonFlightPlan FlightPlan { get; set; }

    [JsonPropertyName("logon_time")]
    public DateTime? LogonTime { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime? LastUpdated { get; set; }
}

public class VatsimJsonPilotRating
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("short_name")]
    public string ShortName { get; set; }

    [JsonPropertyName("long_name")]
    public string LongName { get; set; }
}

public class VatsimJsonPrefile
{
    [JsonPropertyName("cid")]
    public int? Cid { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("callsign")]
    public string Callsign { get; set; }

    [JsonPropertyName("flight_plan")]
    public VatsimJsonFlightPlan FlightPlan { get; set; }

    [JsonPropertyName("last_updated")]
    public DateTime? LastUpdated { get; set; }
}

public class VatsimJsonRating
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("short")]
    public string Short { get; set; }

    [JsonPropertyName("long")]
    public string Long { get; set; }
}

public class VatsimJsonRoot
{
    [JsonPropertyName("general")]
    public VatsimJsonGeneralInfo General { get; set; }

    [JsonPropertyName("pilots")]
    public List<VatsimJsonPilot> Pilots { get; set; }

    [JsonPropertyName("controllers")]
    public List<VatsimJsonController> Controllers { get; set; }

    [JsonPropertyName("atis")]
    public List<VatsimJsonAtis> Atis { get; set; }

    [JsonPropertyName("servers")]
    public List<VatsimJsonServer> Servers { get; set; }

    [JsonPropertyName("prefiles")]
    public List<VatsimJsonPrefile> Prefiles { get; set; }

    [JsonPropertyName("facilities")]
    public List<VatsimJsonFacility> Facilities { get; set; }

    [JsonPropertyName("ratings")]
    public List<VatsimJsonRating> Ratings { get; set; }

    [JsonPropertyName("pilot_ratings")]
    public List<VatsimJsonPilotRating> PilotRatings { get; set; }
}

public class VatsimJsonServer
{
    [JsonPropertyName("ident")]
    public string Ident { get; set; }

    [JsonPropertyName("hostname_or_ip")]
    public string HostnameOrIp { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("clients_connection_allowed")]
    public int? ClientsConnectionAllowed { get; set; }

    [JsonPropertyName("client_connections_allowed")]
    public bool? ClientConnectionsAllowed { get; set; }

    [JsonPropertyName("is_sweatbox")]
    public bool? IsSweatbox { get; set; }
}
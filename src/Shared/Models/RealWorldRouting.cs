using System.Text.Json.Serialization;

namespace ZoaIds.Shared.Models;

public class RealWorldRouting
{
    public string DepartureIcaoId { get; set; }
    public string ArrivalIcaoId { get; set; }
    public ICollection<RouteSummary> RouteSummaries { get; set; }
    
    [JsonIgnore]
    public ICollection<RealWorldFlight> Flights => RouteSummaries.SelectMany(r => r.Flights).ToList();

    public RealWorldRouting(string departureIcaoId, string arrivalIcaoId)
    {
        DepartureIcaoId = departureIcaoId;
        ArrivalIcaoId = arrivalIcaoId;
        RouteSummaries = new List<RouteSummary>();
    }
}

public class RouteSummary
{
	public string DepartureIcaoId { get; set; }
	public string ArrivalIcaoId { get; set; }
	public int RouteFrequency { get; set; }
    public int? MinAltitude { get; set; }
    public int? MaxAltitude { get; set; }
    public string Route { get; set; }
    public int? DistanceMi { get; set; }
    public ICollection<RealWorldFlight> Flights { get; set; }
}

public class RealWorldFlight
{
	public string DepartureIcaoId { get; set; }
	public string ArrivalIcaoId { get; set; }
	public string Callsign { get; set; }
    public string AircraftIcaoId { get; set; }
    public int? Altitude { get; set; }
    public string Route { get; set; }
    public int? Distance { get; set; }
}
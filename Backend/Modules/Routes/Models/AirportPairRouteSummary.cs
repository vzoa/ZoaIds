namespace ZoaIdsBackend.Modules.Routes.Models;

public class AirportPairRouteSummary
{
    public string DepartureIcaoId { get; set; }
    public string ArrivalIcaoId { get; set; }
    public ICollection<FlightRouteSummary> FlightRouteSummaries { get; set; }
    public ICollection<RealWorldFlight> MostRecent { get; set; }

    //[JsonIgnore]
    //public ICollection<RealWorldFlight> Flights => RouteSummaries.SelectMany(r => r.Flights).ToList();

    public AirportPairRouteSummary(string departureIcaoId, string arrivalIcaoId)
    {
        DepartureIcaoId = departureIcaoId;
        ArrivalIcaoId = arrivalIcaoId;
        FlightRouteSummaries = new List<FlightRouteSummary>();
        MostRecent = new List<RealWorldFlight>();
    }
}

public class FlightRouteSummary
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
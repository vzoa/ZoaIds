namespace ZoaIds.Shared.Models;

public class Airport
{
    public string FaaId { get; set; }

    public string? IcaoId { get; set; }
    public string Name { get; set; }
    public AirportType Type { get; set; }
	public int? TrueToMagneticDelta { get; set; }
    public int? MagneticToTrueDelta => TrueToMagneticDelta is null ? null : -1 * TrueToMagneticDelta;
    public Coordinate Location { get; set; }
    public double Elevation { get; set; }
    public string Artcc { get; set; }
    public ICollection<Runway> Runways { get; set; } = new List<Runway>();
    public ICollection<RunwayEnd> RunwayEnds => Runways.SelectMany(x => x.Ends).ToArray();
}

public enum AirportType
{
    Airport,
    Ballonport,
    SeaplaneBase,
    Gliderport,
    Heliport,
    Ultralight
}

public class Runway
{
    public string Name { get; set; }
    public int Length { get; set; }
    public ICollection<RunwayEnd> Ends { get; set; } = new List<RunwayEnd>();
    public string AirportFaaId { get; set; }
}

public class RunwayEnd
{
    public string Name { get; set; }
    public int? TrueHeading { get; set; }
    public int MagneticHeading { get; set; }
    public double? EndElevation { get; set; }
    public double? TdzElevation { get; set; }
	public string RunwayName { get; set; }
    public string AirportFaaId { get; set; }
}

public class Coordinate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    private static readonly double _earthRadiusKm = 6371;

    public Coordinate() { }

    public Coordinate(double lat, double lng)
    {
        Latitude = lat;
        Longitude = lng;
    }

    //public static double SimpleDistanceBetween(Coordinate c1, Coordinate c2, DistanceUnit returnUnit = DistanceUnit.Kilometer)
    //{
    //    return 
    //}

    public enum DistanceUnit
    {
        Kilometer,
        Meter,
        StatuteMile,
        Feet
    }
}


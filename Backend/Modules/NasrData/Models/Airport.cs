using ZoaIdsBackend.Common;

namespace ZoaIdsBackend.Modules.NasrData.Models;

public class Airport
{
    public string FaaId { get; set; }

    public string? IcaoId { get; set; }
    public string Name { get; set; }
    public AirportType Type { get; set; }
    public int? TrueToMagneticDelta { get; set; }
    public int? MagneticToTrueDelta => TrueToMagneticDelta is null ? null : -1 * TrueToMagneticDelta;
    public GeoCoordinate Location { get; set; }
    public double Elevation { get; set; }
    public string Artcc { get; set; }
    public ICollection<Runway> Runways { get; set; } = new List<Runway>();
    public ICollection<Runway.End> RunwayEnds => Runways.SelectMany(x => x.Ends).ToArray();

    public enum AirportType
    {
        Airport,
        Ballonport,
        SeaplaneBase,
        Gliderport,
        Heliport,
        Ultralight
    }
}

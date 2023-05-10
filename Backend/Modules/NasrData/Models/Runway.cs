namespace ZoaIdsBackend.Modules.NasrData.Models;

public class Runway
{
    public string Name { get; set; }
    public int Length { get; set; }
    public ICollection<End> Ends { get; set; } = new List<End>();
    public string AirportFaaId { get; set; }

    public class End
    {
        public string Name { get; set; }
        public int? TrueHeading { get; set; }
        public int MagneticHeading { get; set; }
        public double? EndElevation { get; set; }
        public double? TdzElevation { get; set; }
        public string RunwayName { get; set; }
        public string AirportFaaId { get; set; }
    }
}

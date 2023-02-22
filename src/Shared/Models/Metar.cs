namespace ZoaIds.Shared.Models;

public class Metar
{
    public string StationId { get; set; }
    public string RawText { get; set; }
    public DateTime ObservationTime { get; set; }
    public MetarType Type { get; set; }
    public Coordinate? Location { get; set; }
    public WindObservation? Wind { get; set; }
    public ICollection<SkyCoverObservation>? SkyCovers { get; set; }
    public string? WeatherString { get; set; }
    public float? AltimeterInHg { get; set; }
    public float? TempC { get; set; }
    public float? DewPointC { get; set; }
    public float? VisibilityMi { get; set; }
    public float? SeaLevelPressureMb { get; set; }
    public int? VerticalVisibilityFt { get; set; }

    // Returns the lowest Cloud Cover Base Ft AGL where cover type is BKN or more covered,
    // or Vertical Visibility if the cloud cover is OVX
    public int? Ceiling
    {
        get
        {
            var min = SkyCovers?.Where(c => c.Type >= CloudCoverType.BKN).OrderBy(c => c.BaseFtAgl).FirstOrDefault();
            if (min is null)
            {
                return null;
            }
            else
            {
                return min.Type == CloudCoverType.OVX ? VerticalVisibilityFt : min.BaseFtAgl;
            }
        }
    }
}

public class WindObservation
{
    public int DirectionTrueDegrees { get; set; }
    public int SpeedKnots { get; set; }
    public int? GustKnots { get; set; }
}

public class SkyCoverObservation
{
    public CloudCoverType Type { get; set; }
    public int? BaseFtAgl { get; set; }
}

public enum CloudCoverType
{
    SKC,
    CLR,
    CAVOK,
    FEW,
    SCT,
    BKN,
    OVC,
    OVX
}

public enum MetarType
{
    METAR,
    SPECI
}

public enum FlightRulesCategory
{
    VFR,
    MVFR,
    IFR,
    LIFR
}
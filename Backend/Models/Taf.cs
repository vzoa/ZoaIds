namespace ZoaIdsBackend.Models;
public class Taf
{
    public string StationId { get; set; }
    public string RawText { get; set; }
    public DateTime IssueTime { get; set; }
    public DateTime BulletinTime { get; set; }
    public DateTime ValidFromTime { get; set; }
    public DateTime ValidToTime { get; set; }
    public GeoCoordinate? Location { get; set; }
    public ICollection<Forecast> Forecasts { get; set; }
}

public class Forecast
{
    public DateTime FromTime { get; set; }
    public DateTime ToTime { get; set; }
    public ForecastChangeType ChangeType { get; set; }
    public DateTime? TimeBecoming { get; set; }
    public int? Probability { get; set; }
    public WindObservation? Wind { get; set; }
    public WindShearObservation? WindShear { get; set; }
    public string? WeatherString { get; set; }
    public float? AltimeterInHg { get; set; }
    public float? TempC { get; set; }
    public float? DewPointC { get; set; }
    public float? VisibilityMi { get; set; }
    public int? VerticalVisibilityFt { get; set; }

    // TODO -- turbulence and icing
}

public class WindShearObservation
{
    public int DirectionTrueDegrees { get; set; }
    public int SpeedKnots { get; set; }
    public int HeightFtAgl { get; set; }
}

public enum CloudType
{
    CB,
    TCU,
    CU
}

public enum ForecastChangeType
{
    TEMPO,
    BECMG,
    FM,
    PROB
}

namespace ZoaIdsBackend.Modules.RunwayVisualRange.Models;

public class RvrObservation
{
    public string AirportFaaId { get; set; }
    public string RunwayEndName { get; set; }

    public int? Touchdown { get; set; }
    public RvrTrend? TouchdownTrend { get; set; }

    public int? Midpoint { get; set; }
    public RvrTrend? MidpointTrend { get; set; }

    public int? Rollout { get; set; }
    public RvrTrend? RolloutTrend { get; set; }

    public int? EdgeLightSetting { get; set; }
    public int? CenterlineLightSetting { get; set; }

    public enum RvrTrend
    {
        Decreasing,
        Steady,
        Increasing
    }
}

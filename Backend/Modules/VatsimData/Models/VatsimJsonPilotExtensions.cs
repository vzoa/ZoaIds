using ZoaIdsBackend.Common;
using ZoaIdsBackend.Modules.NasrData.Models;

namespace ZoaIdsBackend.Modules.VatsimData.Models;

public static class VatsimJsonPilotExtensions
{
    public static bool IsOnGroundAtAirport(this VatsimJsonPilot pilot, Airport airport, int maxGroundspeed = 40, int maxElevationDelta = 200, double distanceRangeMiles = 4.0)
    {
        if (pilot.Latitude is null || pilot.Longitude is null)
        {
            return false;
        }
        else
        {
            var pilotCoord = new GeoCoordinate((double)pilot.Latitude, (double)pilot.Longitude);
            return pilot.Groundspeed < maxGroundspeed
                && (pilot.Altitude is not null && Math.Abs((double)pilot.Altitude - airport.Elevation) < maxElevationDelta)
                && pilotCoord.EuclideanDistanceFrom(airport.Location) < distanceRangeMiles;
        }
    }
}

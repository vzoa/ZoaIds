namespace ZoaIdsBackend.Models;

// Source: Jim Speaker, https://stackoverflow.com/questions/46144205/point-in-polygon-using-winding-number

public interface IPolygon
{
    bool Contains(GeoCoordinate location);
}

public class Polygon : IPolygon
{
    private readonly List<GeoCoordinate> _points;

    public Polygon(List<GeoCoordinate> points)
    {
        _points = points;
    }

    public bool Contains(GeoCoordinate location)
    {
        GeoCoordinate[] polygonPointsWithClosure = PolygonPointsWithClosure();

        int windingNumber = 0;

        for (int pointIndex = 0; pointIndex < polygonPointsWithClosure.Length - 1; pointIndex++)
        {
            var edge = new Edge(polygonPointsWithClosure[pointIndex], polygonPointsWithClosure[pointIndex + 1]);
            windingNumber += AscendingIntersection(location, edge);
            windingNumber -= DescendingIntersection(location, edge);
        }

        return windingNumber != 0;
    }

    private GeoCoordinate[] PolygonPointsWithClosure()
    {
        // _points should remain immutable, thus creation of a closed point set (starting point repeated)
        return new List<GeoCoordinate>(_points)
        {
            new GeoCoordinate(_points[0].Latitude, _points[0].Longitude)
        }.ToArray();
    }

    private static int AscendingIntersection(GeoCoordinate location, Edge edge)
    {
        if (!edge.AscendingRelativeTo(location)) { return 0; }

        if (!edge.LocationInRange(location, Orientation.Ascending)) { return 0; }

        return Wind(location, edge, Position.Left);
    }

    private static int DescendingIntersection(GeoCoordinate location, Edge edge)
    {
        if (edge.AscendingRelativeTo(location)) { return 0; }

        if (!edge.LocationInRange(location, Orientation.Descending)) { return 0; }

        return Wind(location, edge, Position.Right);
    }

    private static int Wind(GeoCoordinate location, Edge edge, Position position)
    {
        if (edge.RelativePositionOf(location) != position) { return 0; }

        return 1;
    }

    private class Edge
    {
        private readonly GeoCoordinate _startPoint;
        private readonly GeoCoordinate _endPoint;

        public Edge(GeoCoordinate startPoint, GeoCoordinate endPoint)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
        }

        public Position RelativePositionOf(GeoCoordinate location)
        {
            double positionCalculation =
                (_endPoint.Longitude - _startPoint.Longitude) * (location.Latitude - _startPoint.Latitude) -
                (location.Longitude - _startPoint.Longitude) * (_endPoint.Latitude - _startPoint.Latitude);

            if (positionCalculation > 0) { return Position.Left; }

            if (positionCalculation < 0) { return Position.Right; }

            return Position.Center;
        }

        public bool AscendingRelativeTo(GeoCoordinate location)
        {
            return _startPoint.Latitude <= location.Latitude;
        }

        public bool LocationInRange(GeoCoordinate location, Orientation orientation)
        {
            if (orientation == Orientation.Ascending) return _endPoint.Latitude > location.Latitude;

            return _endPoint.Latitude <= location.Latitude;
        }
    }

    private enum Position
    {
        Left,
        Right,
        Center
    }

    private enum Orientation
    {
        Ascending,
        Descending
    }
}
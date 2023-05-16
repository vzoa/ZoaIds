using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZoaIdsBackend.Common;

public class GeoCoordinate
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    private const double _earthRadiusKm = 6371;
    private const double _degToRadConversionFactor = Math.PI / 180.0;

    public GeoCoordinate() { }

    public GeoCoordinate(double lat, double lng)
    {
        Latitude = lat;
        Longitude = lng;
    }

    public double EuclideanDistanceFrom(GeoCoordinate otherCoord, DistanceUnit returnUnit = DistanceUnit.StatuteMile)
    {
        return EuclideanDistanceBetween(this, otherCoord, returnUnit);
    }

    public static double EuclideanDistanceBetween(GeoCoordinate c1, GeoCoordinate c2, DistanceUnit returnUnit = DistanceUnit.StatuteMile)
    {
        var c1Rad = new GeoCoordinate(c1.Latitude * _degToRadConversionFactor, c1.Longitude * _degToRadConversionFactor);
        var c2Rad = new GeoCoordinate(c2.Latitude * _degToRadConversionFactor, c2.Longitude * _degToRadConversionFactor);
        double unitlessDistance = Math.Sqrt(2) *
            Math.Sqrt(1
                - (Math.Cos(c1Rad.Latitude) * Math.Cos(c2Rad.Latitude) * Math.Cos(c1Rad.Longitude - c2Rad.Longitude))
                - (Math.Sin(c1Rad.Latitude) * Math.Sin(c2Rad.Latitude)));

        var unitConversionFactor = returnUnit switch
        {
            DistanceUnit.Kilometer => 1,
            DistanceUnit.Meter => 1000,
            DistanceUnit.StatuteMile => 0.6213711922,
            DistanceUnit.NauticalMile => 0.539957,
            DistanceUnit.Feet => 3280.84,
            _ => throw new NotImplementedException()
        };

        return unitlessDistance * unitConversionFactor * _earthRadiusKm;
    }

    public enum DistanceUnit
    {
        Kilometer,
        Meter,
        StatuteMile,
        NauticalMile,
        Feet
    }
}

public class GeoCoordinateJsonConverter : JsonConverter<GeoCoordinate>
{
    public override GeoCoordinate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray) { throw new JsonException("Expected StartArray token"); }

        reader.Read();
        if (reader.TokenType != JsonTokenType.Number) { throw new JsonException("Longitude must be a number"); }
        var lng = reader.GetDouble();

        reader.Read();
        if (reader.TokenType != JsonTokenType.Number) { throw new JsonException("Latitude nust be a number"); }
        var lat = reader.GetDouble();

        reader.Read();
        if (reader.TokenType != JsonTokenType.EndArray) { throw new JsonException("Expected EndArray token"); }

        return new GeoCoordinate(lat, lng);
    }

    public override void Write(Utf8JsonWriter writer, GeoCoordinate value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.Longitude);
        writer.WriteNumberValue(value.Latitude);
        writer.WriteEndArray();
    }
}

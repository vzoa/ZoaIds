namespace ZoaIds.Shared.Models;

public class Coordinate
{
	public double Latitude { get; set; }
	public double Longitude { get; set; }

	private const double _earthRadiusKm = 6371;
	private const double _degToRadConversionFactor = Math.PI / 180.0;

	public Coordinate() { }

	public Coordinate(double lat, double lng)
	{
		Latitude = lat;
		Longitude = lng;
	}

	public static double EuclideanDistanceBetween(Coordinate c1, Coordinate c2, DistanceUnit returnUnit = DistanceUnit.StatuteMile)
	{
		var c1Rad = new Coordinate(c1.Latitude * _degToRadConversionFactor, c1.Longitude * _degToRadConversionFactor);
		var c2Rad = new Coordinate(c2.Latitude * _degToRadConversionFactor, c2.Longitude * _degToRadConversionFactor);
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
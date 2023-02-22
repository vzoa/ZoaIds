namespace ZoaIds.Server;

public static class Constants
{
	public static class Urls
	{
		public const string AircraftIcaoCsv = "https://raw.githubusercontent.com/kengreim/Zoa-Info-Tool/main/Assets/Data/v1/aircraft.csv";
		public const string AirlinesIcaoCsv = "https://raw.githubusercontent.com/kengreim/Zoa-Info-Tool/main/Assets/Data/v1/airlines.csv";
		public const string ZoaAliasFile = "https://raw.githubusercontent.com/kengreim/Zoa-Info-Tool/main/Assets/Data/v1/ZOA_Alias.txt";
		public const string MetarsXml = "https://aviationweather.gov/adds/dataserver_current/current/metars.cache.xml";
		public const string NasrApiEndpoint = "https://soa.smext.faa.gov/apra/nfdc/nasr/chart?edition=current";
		public const string ZoaProcedures = "https://oakartcc.org/controllers/procedures";
		public const string ClowdDatisApiEndpoint = "https://datis.clowd.io/api/";
		public const string VatsimDatafeedBase = "https://data.vatsim.net/v3/";
		public const string ChartsApiEndpoint = "https://api.aviationapi.com/v1/charts";
		public const string FlightAwareIfrRouteBase = "https://flightaware.com/analysis/route.rvt?";
		public const string FaaRvrBase = "https://rvr.data.faa.gov/";
		public const string FaaRvrAirportLookup = "https://rvr.data.faa.gov/cgi-bin/rvr-airports.pl";
	}

	public const int VatsimDatafeedUpdateFrequencySeconds = 15;
	public const int DatisRefreshDelaySeconds = 60;
	public const int VatsimDataKeepForfHours = 24;
	public const int RoutesCacheTtlSeconds = 1200;
	public const int RvrUpdateIntervalSeconds = 120;

	public const string ZoaDocumentsPdfPath = "zoapdfs";

	public static readonly string[] RvrAirportIds = { "FAT", "OAK", "RNO", "SFO", "SJC", "SMF" };
}

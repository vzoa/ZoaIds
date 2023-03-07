namespace ZoaIds.Client;

public static class Constants
{
	public static readonly string[] ZoaClassB = { "KSFO" };
	public static readonly string[] ZoaClassC = { "KOAK", "KSJC", "KSMF", "KRNO", "KFAT", "KMRY" };
	public static readonly string[] ZoaClassD = { "KAPC", "KCCR", "KCIC", "KHWD", "KLVK", "KMER", "KMHR", "KMOD",
		"KNUQ", "KPAO", "KRDD", "KRHV", "KSAC", "KSCK", "KSNS", "KSQL", "KSTS", "KTRK" };

	public const string ApiBase = "api/v1/";
	public static class ApiEndpoints
	{
		public const string Vatsim = "vatsim";
		public const string Airports = "airports";
		public const string Charts = "charts";
		public const string ZoaDocuments = "zoadocuments";
		public const string Datis = "datis";
		public const string Weather = "weather";
		public const string AliasRoutes = "aliasroutes";
		public const string RealWorldRoutes = "realworldroutes";
	}
}

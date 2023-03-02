using ZoaIds.Shared.ExternalDataModels;

namespace ZoaIds.Shared.Extensions;

public static class IVatsimJsonControlConnectionExtensions
{
	public static string SingleLineAtis(this IVatsimControlConnection vatsimControlConnection)
	{
		return string.Join(" ", vatsimControlConnection.TextAtis);
	}
}

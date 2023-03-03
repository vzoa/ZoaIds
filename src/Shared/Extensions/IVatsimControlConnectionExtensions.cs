using ZoaIds.Shared.ExternalDataModels;

namespace ZoaIds.Shared.Extensions;

public static class IVatsimJsonControlConnectionExtensions
{
	public static string SingleLineAtis(this IVatsimControlConnection vatsimControlConnection)
	{
		if (vatsimControlConnection.TextAtis is null)
		{
			return string.Empty;
		}
		else
		{
			return string.Join(" ", vatsimControlConnection.TextAtis);

		}
	}
}

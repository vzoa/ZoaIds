using ZoaIds.Shared.ExternalDataModels;

namespace ZoaIds.Shared.Extensions;

public static class VatsimJsonAtisExtensions
{
	public static string SingleLineAtis(this VatsimJsonAtis vatsimJsonAtis)
	{
		return string.Join(" ", vatsimJsonAtis.TextAtis);
	}
}

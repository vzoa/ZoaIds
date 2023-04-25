using ZoaIdsBackend.ExternalDataModels;

namespace ZoaIdsBackend.Extensions;

public static class IVatsimJsonControlConnectionExtensions
{
    public static string SingleLineAtis(this IVatsimControlConnection vatsimControlConnection)
    {
        return (vatsimControlConnection.TextAtis is null)
            ? string.Empty
            : string.Join(" ", vatsimControlConnection.TextAtis);
    }
}

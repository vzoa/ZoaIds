namespace ZoaIdsBackend.Services;

public interface IRouteSummaryService
{
    public Task<RealWorldRouting> FetchRoutesAsync(string departureIcao, string arrivalIcao);
}

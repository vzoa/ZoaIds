using ZoaIds.Shared.Models;

namespace ZoaIds.Server.Services;

public interface IRouteSummaryService
{
    public Task<RealWorldRouting> FetchRoutesAsync(string departureIcao, string arrivalIcao);
}

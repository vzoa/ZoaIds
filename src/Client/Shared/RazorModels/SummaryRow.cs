using ZoaIds.Shared.ExternalDataModels;

namespace ZoaIds.Client.Shared.RazorModels;

public class SummaryRow
{
    public string Airport { get; set; }
    public int Count { get; set; }
    public IEnumerable<IVatsimPilotActivity> Flights { get; set; }

    public SummaryRow(string airport, IEnumerable<IVatsimPilotActivity> flights)
    {
        Airport = airport;
        Flights = flights;
        Count = flights.Count();
    }
}

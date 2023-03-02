using ZoaIds.Shared.ExternalDataModels;
using ZoaIds.Shared.Models;

namespace ZoaIds.Shared.Extensions;

public static class VatsimJsonRootExtensions
{
	public static IEnumerable<VatsimJsonAtis> GetAtis(this VatsimJsonRoot vatsimJsonRoot, string airportIcaoId)
	{
		return vatsimJsonRoot.Atis.Where(a => a.Callsign[..4].Equals(airportIcaoId, StringComparison.OrdinalIgnoreCase));
	}

	public static IEnumerable<VatsimJsonAtis> GetAtis(this VatsimJsonRoot vatsimJsonRoot, Airport airport)
	{
		return airport.IcaoId is not null ? GetAtis(vatsimJsonRoot, airport.IcaoId) : Enumerable.Empty<VatsimJsonAtis>();
	}

	public static IEnumerable<VatsimJsonPilot> GetActiveDeparturesFrom(this VatsimJsonRoot vatsimJsonRoot, string airportIcaoId)
	{
		return vatsimJsonRoot.Pilots.Where(p => p.FlightPlan is not null && p.FlightPlan.Departure.Equals(airportIcaoId, StringComparison.OrdinalIgnoreCase));
	}

	public static IEnumerable<VatsimJsonPilot> GetActiveDeparturesFrom(this VatsimJsonRoot vatsimJsonRoot, Airport airport)
	{
		return airport.IcaoId is not null ? GetActiveDeparturesFrom(vatsimJsonRoot, airport.IcaoId) : Enumerable.Empty<VatsimJsonPilot>();
	}

	public static IEnumerable<VatsimJsonPilot> GetActiveArrivalsTo(this VatsimJsonRoot vatsimJsonRoot, string airportIcaoId)
	{
		return vatsimJsonRoot.Pilots.Where(p => p.FlightPlan is not null && p.FlightPlan.Arrival.Equals(airportIcaoId, StringComparison.OrdinalIgnoreCase));
	}

	public static IEnumerable<VatsimJsonPrefile> GetActiveArrivalsTo(this VatsimJsonRoot vatsimJsonRoot, Airport airport)
	{
		return airport.IcaoId is not null ? GetPrefiledArrivalsTo(vatsimJsonRoot, airport.IcaoId) : Enumerable.Empty<VatsimJsonPrefile>();
	}

	public static IEnumerable<VatsimJsonPrefile> GetPrefiledDeparturesFrom(this VatsimJsonRoot vatsimJsonRoot, string airportIcaoId)
	{
		return vatsimJsonRoot.Prefiles.Where(p => p.FlightPlan is not null && p.FlightPlan.Departure.Equals(airportIcaoId, StringComparison.OrdinalIgnoreCase));
	}

	public static IEnumerable<VatsimJsonPrefile> GetPrefiledDeparturesFrom(this VatsimJsonRoot vatsimJsonRoot, Airport airport)
	{
		return airport.IcaoId is not null ? GetPrefiledDeparturesFrom(vatsimJsonRoot, airport.IcaoId) : Enumerable.Empty<VatsimJsonPrefile>();
	}

	public static IEnumerable<VatsimJsonPrefile> GetPrefiledArrivalsTo(this VatsimJsonRoot vatsimJsonRoot, string airportIcaoId)
	{
		return vatsimJsonRoot.Prefiles.Where(p => p.FlightPlan is not null && p.FlightPlan.Arrival.Equals(airportIcaoId, StringComparison.OrdinalIgnoreCase));
	}

	public static IEnumerable<VatsimJsonPrefile> GetPrefiledArrivalsTo(this VatsimJsonRoot vatsimJsonRoot, Airport airport)
	{
		return airport.IcaoId is not null ? GetPrefiledArrivalsTo(vatsimJsonRoot, airport.IcaoId) : Enumerable.Empty<VatsimJsonPrefile>();
	}

    public static IEnumerable<VatsimJsonController> GetControllersByPrefix(this VatsimJsonRoot vatsimJsonRoot, string airportPrefix)
    {
        return vatsimJsonRoot.Controllers.Where(c => c.Callsign.StartsWith(airportPrefix, StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<VatsimJsonController> GetControllersBySuffix(this VatsimJsonRoot vatsimJsonRoot, string airportSuffix)
    {
        return vatsimJsonRoot.Controllers.Where(c => c.Callsign.EndsWith(airportSuffix, StringComparison.OrdinalIgnoreCase));
    }
}

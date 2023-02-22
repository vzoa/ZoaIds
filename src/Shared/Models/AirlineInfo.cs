namespace ZoaIds.Shared.Models;

public class AirlineInfo
{
	public string IcaoId { get; set; }
	public string? Callsign { get; set; }
	public string Name { get; set; }
	public string Country { get; set; }

	public AirlineInfo() { }

	public AirlineInfo(string icaoID, string? callsign, string name)
	{
		IcaoId = icaoID;
		Callsign = callsign;
		Name = name;
	}
}

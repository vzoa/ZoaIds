namespace ZoaIdsBackend.Modules.VatsimData.Models;

public class Artcc
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string Id { get; set; }
    public bool IsOceanic { get; set; }
    public string? Region { get; set; }
    public string Division { get; set; }
    public string SerializedBoundingPolygons { get; set; }
}

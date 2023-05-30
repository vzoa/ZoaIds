using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Common;
using ZoaIdsBackend.Modules.AircraftTypes.Models;
using ZoaIdsBackend.Modules.Airlines.Models;
using ZoaIdsBackend.Modules.DigitalAtis.Models;
using ZoaIdsBackend.Modules.NasrData.Models;
using ZoaIdsBackend.Modules.RunwayVisualRange.Models;
using ZoaIdsBackend.Modules.VatsimData.Models;
using ZoaIdsBackend.Modules.Weather.Models;

namespace ZoaIdsBackend.Data;

public class ZoaIdsContext : DbContext
{
    public DbSet<VatsimSnapshot> VatsimSnapshots { get; set; }
    public DbSet<Airport> Airports { get; set; }
    public DbSet<ApplicationJobRecord> CompletedJobs { get; set; }
    public DbSet<Atis> Atises { get; set; }
    public DbSet<Airline> Airlines { get; set; }
    public DbSet<AircraftType> AircraftTypes { get; set; }
    public DbSet<Metar> Metars { get; set; }
    public DbSet<RvrObservation> RvrObservations { get; set; }
    public DbSet<Artcc> Artccs { get; set; }
    //public DbSet<AirportPairRouteSummary> RealWorldRoutings { get; set; }


    public ZoaIdsContext(DbContextOptions<ZoaIdsContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Scans assembly for all classes of IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ZoaIdsContext).Assembly);
        
        //modelBuilder.Entity<AirportPairRouteSummary>()
        //    .HasKey(r => new { r.DepartureIcaoId, r.ArrivalIcaoId });

        //modelBuilder.Entity<AirportPairRouteSummary>().Property<DateTime>("Created").HasDefaultValueSql("CURRENT_TIMESTAMP");

        //modelBuilder.Entity<AirportPairRouteSummary>().OwnsMany(
        //    r => r.FlightRouteSummaries, s =>
        //    {
        //        s.WithOwner().HasForeignKey(s => new { s.DepartureIcaoId, s.ArrivalIcaoId });
        //        s.HasKey(s => new { s.DepartureIcaoId, s.Route, s.ArrivalIcaoId });
        //        s.OwnsMany(
        //            s => s.Flights, f =>
        //            {
        //                f.WithOwner().HasForeignKey(f => new { f.DepartureIcaoId, f.Route, f.ArrivalIcaoId });
        //                f.Property<int>("Id");
        //                f.HasKey("Id");
        //            });
        //    });
    }
}

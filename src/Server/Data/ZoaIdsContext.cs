using Microsoft.EntityFrameworkCore;
using ZoaIds.Shared.Models;

namespace ZoaIds.Server.Data;

public class ZoaIdsContext : DbContext
{
    public DbSet<VatsimSnapshot> VatsimSnapshots { get; set; }
    public DbSet<Airport> Airports { get; set; }
    public DbSet<Atis> Atises { get; set; }
    public DbSet<AircraftTypeInfo> AircraftTypes { get; set; }
    public DbSet<AirlineInfo> Airlines { get; set; }

    public DbSet<Metar> Metars { get; set; }

    public DbSet<ArtccDocument> ZoaDocuments { get; set; }

    public DbSet<RealWorldRouting> RealWorldRoutings { get; set; }

    public DbSet<RouteRule> AliasRouteRules { get; set; }

    public DbSet<RvrObservation> RvrObservations { get; set; }

    // Application Data
    public DbSet<ApplicationJob> CompletedJobs { get; set; }


    public ZoaIdsContext(DbContextOptions<ZoaIdsContext> options) : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airport>()
            .Ignore(a => a.RunwayEnds)
            .HasKey(a => a.FaaId);

        modelBuilder.Entity<Airport>().OwnsOne(a => a.Location);

        modelBuilder.Entity<Airport>().OwnsMany(
            a => a.Runways, r =>
            {
				r.WithOwner().HasForeignKey("AirportFaaId");
				r.Property<int>("Id");
				r.HasKey("Id");
                r.OwnsMany(
                    r => r.Ends, e =>
                    {
                        e.WithOwner().HasForeignKey("RunwayOwnerId");
                        e.Property<int>("Id");
                        e.HasKey("Id");
                    });
			});


        // Atis needs a composite primary key because a given airport can
        // have both a departure and an arrival ATIS
        modelBuilder.Entity<Atis>()
            .HasKey(a => new { a.IcaoId, a.Type });

        // Aircraft Type Info needs a shadow property primary key because
        // there can be multiple entries for a single aircraft ICAO code
        modelBuilder.Entity<AircraftTypeInfo>()
            .Property<int>("AircraftTypeId")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<AircraftTypeInfo>().HasKey("AircraftTypeId");

        modelBuilder.Entity<AirlineInfo>().HasKey(a => a.IcaoId);


        // Set up owned entities for Metar
		modelBuilder.Entity<Metar>().HasKey(m => m.StationId);
        modelBuilder.Entity<Metar>().OwnsOne(m => m.Location);
		modelBuilder.Entity<Metar>().OwnsOne(m => m.Wind);
        modelBuilder.Entity<Metar>().OwnsMany(
            m => m.SkyCovers, s =>
            {
			    s.WithOwner().HasForeignKey("StationId");
			    s.Property<int>("SkyCoverId");
                s.HasKey("SkyCoverId");
            });

        modelBuilder.Entity<ArtccDocument>().Property<int>("Id");
        modelBuilder.Entity<ArtccDocument>().HasKey("Id");


		// Entities for Real World Routing

		modelBuilder.Entity<RealWorldRouting>()
            .Ignore(r => r.Flights)
            .HasKey(r => new { r.DepartureIcaoId, r.ArrivalIcaoId });

        modelBuilder.Entity<RealWorldRouting>().Property<DateTime>("Created").HasDefaultValueSql("CURRENT_TIMESTAMP");

		modelBuilder.Entity<RealWorldRouting>().OwnsMany(
            r => r.RouteSummaries, s =>
            {
                s.WithOwner().HasForeignKey(s => new {s.DepartureIcaoId, s.ArrivalIcaoId });
                s.HasKey(s => new {s.DepartureIcaoId, s.Route, s.ArrivalIcaoId });
                s.OwnsMany(
                    s => s.Flights, f =>
                    {
                        f.WithOwner().HasForeignKey(f => new {f.DepartureIcaoId, f.Route, f.ArrivalIcaoId });
                        f.Property<int>("Id");
                        f.HasKey("Id");
                    });
            });


        modelBuilder.Entity<RouteRule>().Property<int>("Id");
        modelBuilder.Entity<RouteRule>().HasKey("Id");

        modelBuilder.Entity<RvrObservation>().Property<int>("Id");
		modelBuilder.Entity<RvrObservation>().HasKey("Id");


		//modelBuilder.Entity<Atis>()
		//    .Property(a => a.UniqueId)
		//    .HasColumnName("IcaoPlusType")
		//    .IsRequired(true);
		//modelBuilder.Entity<Airport>()
		//    .OwnsMany(a => a.Runways, r =>
		//    {
		//        r.
		//    }
	}
}

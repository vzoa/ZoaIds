﻿using Microsoft.EntityFrameworkCore;
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


        modelBuilder.Entity<Airline>().HasKey(a => a.IcaoId);

        // Aircraft Type Info needs a shadow property primary key because
        // there can be multiple entries for a single aircraft ICAO code
        modelBuilder.Entity<AircraftType>()
            .Property<int>("AircraftTypeId")
            .ValueGeneratedOnAdd();


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

        modelBuilder.Entity<RvrObservation>().Property<int>("Id");
        modelBuilder.Entity<RvrObservation>().HasKey("Id");

        modelBuilder.Entity<Artcc>().HasKey(a => a.Guid);
    }
}

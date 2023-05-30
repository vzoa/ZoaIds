﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZoaIdsBackend.Data;

#nullable disable

namespace ZoaIdsBackend.Migrations
{
    [DbContext(typeof(ZoaIdsContext))]
    [Migration("20230530184917_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("ZoaIdsBackend.Common.ApplicationJobRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Caller")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("JobKey")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("JobValue")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CompletedJobs");
                });

            modelBuilder.Entity("ZoaIdsBackend.Modules.AircraftTypes.Models.AircraftType", b =>
                {
                    b.Property<int>("AircraftTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Class")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ConslidatedWakeTurbulenceCategory")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("EngineCount")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("EngineType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FaaEngineNumberType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FaaWeightClass")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("IcaoId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("IcaoWakeTurbulenceCategory")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LandAndHoldShortGroup")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SameRunwaySeparationCategory")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AircraftTypeId");

                    b.ToTable("AircraftTypes");
                });

            modelBuilder.Entity("ZoaIdsBackend.Modules.Airlines.Models.Airline", b =>
                {
                    b.Property<string>("IcaoId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Callsign")
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IcaoId");

                    b.ToTable("Airlines");
                });

            modelBuilder.Entity("ZoaIdsBackend.Modules.DigitalAtis.Models.Atis", b =>
                {
                    b.Property<string>("IcaoId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Altimeter")
                        .HasColumnType("INTEGER");

                    b.Property<char>("InfoLetter")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("IssueTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("RawText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("StatusText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UniqueId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("WeatherText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("IcaoId", "Type");

                    b.ToTable("Atises");
                });

            modelBuilder.Entity("ZoaIdsBackend.Modules.NasrData.Models.Airport", b =>
                {
                    b.Property<string>("FaaId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Artcc")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Elevation")
                        .HasColumnType("REAL");

                    b.Property<string>("IcaoId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("TrueToMagneticDelta")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("FaaId");

                    b.ToTable("Airports");
                });

            modelBuilder.Entity("ZoaIdsBackend.Modules.RunwayVisualRange.Models.RvrObservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AirportFaaId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("CenterlineLightSetting")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EdgeLightSetting")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Midpoint")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("MidpointTrend")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Rollout")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("RolloutTrend")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RunwayEndName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("Touchdown")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TouchdownTrend")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("RvrObservations");
                });

            modelBuilder.Entity("ZoaIdsBackend.Modules.VatsimData.Models.Artcc", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Division")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsOceanic")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Region")
                        .HasColumnType("TEXT");

                    b.Property<string>("SerializedBoundingPolygons")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Guid");

                    b.ToTable("Artccs");
                });

            modelBuilder.Entity("ZoaIdsBackend.Modules.VatsimData.Models.VatsimSnapshot", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("RawJson")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("VatsimSnapshots");
                });

            modelBuilder.Entity("ZoaIdsBackend.Modules.Weather.Models.Metar", b =>
                {
                    b.Property<string>("StationId")
                        .HasColumnType("TEXT");

                    b.Property<float?>("AltimeterInHg")
                        .HasColumnType("REAL");

                    b.Property<float?>("DewPointC")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("ObservationTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("RawText")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<float?>("SeaLevelPressureMb")
                        .HasColumnType("REAL");

                    b.Property<float?>("TempC")
                        .HasColumnType("REAL");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("VerticalVisibilityFt")
                        .HasColumnType("INTEGER");

                    b.Property<float?>("VisibilityMi")
                        .HasColumnType("REAL");

                    b.Property<string>("WeatherString")
                        .HasColumnType("TEXT");

                    b.HasKey("StationId");

                    b.ToTable("Metars");
                });

            modelBuilder.Entity("ZoaIdsBackend.Modules.NasrData.Models.Airport", b =>
                {
                    b.OwnsOne("ZoaIdsBackend.Common.GeoCoordinate", "Location", b1 =>
                        {
                            b1.Property<string>("AirportFaaId")
                                .HasColumnType("TEXT");

                            b1.Property<double>("Latitude")
                                .HasColumnType("REAL");

                            b1.Property<double>("Longitude")
                                .HasColumnType("REAL");

                            b1.HasKey("AirportFaaId");

                            b1.ToTable("Airports");

                            b1.WithOwner()
                                .HasForeignKey("AirportFaaId");
                        });

                    b.OwnsMany("ZoaIdsBackend.Modules.NasrData.Models.Runway", "Runways", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER");

                            b1.Property<string>("AirportFaaId")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<int>("Length")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("Id");

                            b1.HasIndex("AirportFaaId");

                            b1.ToTable("Runway");

                            b1.WithOwner()
                                .HasForeignKey("AirportFaaId");

                            b1.OwnsMany("ZoaIdsBackend.Modules.NasrData.Models.Runway+End", "Ends", b2 =>
                                {
                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("AirportFaaId")
                                        .IsRequired()
                                        .HasColumnType("TEXT");

                                    b2.Property<double?>("EndElevation")
                                        .HasColumnType("REAL");

                                    b2.Property<int>("MagneticHeading")
                                        .HasColumnType("INTEGER");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasColumnType("TEXT");

                                    b2.Property<string>("RunwayName")
                                        .IsRequired()
                                        .HasColumnType("TEXT");

                                    b2.Property<int>("RunwayOwnerId")
                                        .HasColumnType("INTEGER");

                                    b2.Property<double?>("TdzElevation")
                                        .HasColumnType("REAL");

                                    b2.Property<int?>("TrueHeading")
                                        .HasColumnType("INTEGER");

                                    b2.HasKey("Id");

                                    b2.HasIndex("RunwayOwnerId");

                                    b2.ToTable("End");

                                    b2.WithOwner()
                                        .HasForeignKey("RunwayOwnerId");
                                });

                            b1.Navigation("Ends");
                        });

                    b.Navigation("Location")
                        .IsRequired();

                    b.Navigation("Runways");
                });

            modelBuilder.Entity("ZoaIdsBackend.Modules.Weather.Models.Metar", b =>
                {
                    b.OwnsOne("ZoaIdsBackend.Common.GeoCoordinate", "Location", b1 =>
                        {
                            b1.Property<string>("MetarStationId")
                                .HasColumnType("TEXT");

                            b1.Property<double>("Latitude")
                                .HasColumnType("REAL");

                            b1.Property<double>("Longitude")
                                .HasColumnType("REAL");

                            b1.HasKey("MetarStationId");

                            b1.ToTable("Metars");

                            b1.WithOwner()
                                .HasForeignKey("MetarStationId");
                        });

                    b.OwnsOne("ZoaIdsBackend.Modules.Weather.Models.WindObservation", "Wind", b1 =>
                        {
                            b1.Property<string>("MetarStationId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("DirectionTrueDegrees")
                                .HasColumnType("INTEGER");

                            b1.Property<int?>("GustKnots")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("SpeedKnots")
                                .HasColumnType("INTEGER");

                            b1.HasKey("MetarStationId");

                            b1.ToTable("Metars");

                            b1.WithOwner()
                                .HasForeignKey("MetarStationId");
                        });

                    b.OwnsMany("ZoaIdsBackend.Modules.Weather.Models.SkyCoverObservation", "SkyCovers", b1 =>
                        {
                            b1.Property<int>("SkyCoverId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("INTEGER");

                            b1.Property<int?>("BaseFtAgl")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("StationId")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<int>("Type")
                                .HasColumnType("INTEGER");

                            b1.HasKey("SkyCoverId");

                            b1.HasIndex("StationId");

                            b1.ToTable("SkyCoverObservation");

                            b1.WithOwner()
                                .HasForeignKey("StationId");
                        });

                    b.Navigation("Location");

                    b.Navigation("SkyCovers");

                    b.Navigation("Wind");
                });
#pragma warning restore 612, 618
        }
    }
}
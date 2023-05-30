using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZoaIdsBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AircraftTypes",
                columns: table => new
                {
                    AircraftTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IcaoId = table.Column<string>(type: "TEXT", nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    Class = table.Column<string>(type: "TEXT", nullable: false),
                    EngineType = table.Column<string>(type: "TEXT", nullable: false),
                    EngineCount = table.Column<string>(type: "TEXT", nullable: false),
                    IcaoWakeTurbulenceCategory = table.Column<string>(type: "TEXT", nullable: false),
                    FaaEngineNumberType = table.Column<string>(type: "TEXT", nullable: false),
                    FaaWeightClass = table.Column<string>(type: "TEXT", nullable: false),
                    ConslidatedWakeTurbulenceCategory = table.Column<string>(type: "TEXT", nullable: false),
                    SameRunwaySeparationCategory = table.Column<string>(type: "TEXT", nullable: false),
                    LandAndHoldShortGroup = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AircraftTypes", x => x.AircraftTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Airlines",
                columns: table => new
                {
                    IcaoId = table.Column<string>(type: "TEXT", nullable: false),
                    Callsign = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airlines", x => x.IcaoId);
                });

            migrationBuilder.CreateTable(
                name: "Airports",
                columns: table => new
                {
                    FaaId = table.Column<string>(type: "TEXT", nullable: false),
                    IcaoId = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    TrueToMagneticDelta = table.Column<int>(type: "INTEGER", nullable: true),
                    Location_Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Location_Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Elevation = table.Column<double>(type: "REAL", nullable: false),
                    Artcc = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airports", x => x.FaaId);
                });

            migrationBuilder.CreateTable(
                name: "Artccs",
                columns: table => new
                {
                    Guid = table.Column<Guid>(type: "TEXT", nullable: false),
                    Id = table.Column<string>(type: "TEXT", nullable: true),
                    IsOceanic = table.Column<bool>(type: "INTEGER", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: true),
                    Division = table.Column<string>(type: "TEXT", nullable: false),
                    SerializedBoundingPolygons = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artccs", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Atises",
                columns: table => new
                {
                    IcaoId = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    InfoLetter = table.Column<char>(type: "TEXT", nullable: false),
                    IssueTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Altimeter = table.Column<int>(type: "INTEGER", nullable: false),
                    RawText = table.Column<string>(type: "TEXT", nullable: false),
                    WeatherText = table.Column<string>(type: "TEXT", nullable: false),
                    StatusText = table.Column<string>(type: "TEXT", nullable: false),
                    UniqueId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atises", x => new { x.IcaoId, x.Type });
                });

            migrationBuilder.CreateTable(
                name: "CompletedJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Caller = table.Column<string>(type: "TEXT", nullable: false),
                    JobKey = table.Column<string>(type: "TEXT", nullable: false),
                    JobValue = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Metars",
                columns: table => new
                {
                    StationId = table.Column<string>(type: "TEXT", nullable: false),
                    RawText = table.Column<string>(type: "TEXT", nullable: false),
                    ObservationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Location_Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Location_Longitude = table.Column<double>(type: "REAL", nullable: true),
                    Wind_DirectionTrueDegrees = table.Column<int>(type: "INTEGER", nullable: true),
                    Wind_SpeedKnots = table.Column<int>(type: "INTEGER", nullable: true),
                    Wind_GustKnots = table.Column<int>(type: "INTEGER", nullable: true),
                    WeatherString = table.Column<string>(type: "TEXT", nullable: true),
                    AltimeterInHg = table.Column<float>(type: "REAL", nullable: true),
                    TempC = table.Column<float>(type: "REAL", nullable: true),
                    DewPointC = table.Column<float>(type: "REAL", nullable: true),
                    VisibilityMi = table.Column<float>(type: "REAL", nullable: true),
                    SeaLevelPressureMb = table.Column<float>(type: "REAL", nullable: true),
                    VerticalVisibilityFt = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metars", x => x.StationId);
                });

            migrationBuilder.CreateTable(
                name: "RvrObservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AirportFaaId = table.Column<string>(type: "TEXT", nullable: false),
                    RunwayEndName = table.Column<string>(type: "TEXT", nullable: false),
                    Touchdown = table.Column<int>(type: "INTEGER", nullable: true),
                    TouchdownTrend = table.Column<int>(type: "INTEGER", nullable: true),
                    Midpoint = table.Column<int>(type: "INTEGER", nullable: true),
                    MidpointTrend = table.Column<int>(type: "INTEGER", nullable: true),
                    Rollout = table.Column<int>(type: "INTEGER", nullable: true),
                    RolloutTrend = table.Column<int>(type: "INTEGER", nullable: true),
                    EdgeLightSetting = table.Column<int>(type: "INTEGER", nullable: true),
                    CenterlineLightSetting = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RvrObservations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VatsimSnapshots",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RawJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VatsimSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Runway",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Length = table.Column<int>(type: "INTEGER", nullable: false),
                    AirportFaaId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runway", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Runway_Airports_AirportFaaId",
                        column: x => x.AirportFaaId,
                        principalTable: "Airports",
                        principalColumn: "FaaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkyCoverObservation",
                columns: table => new
                {
                    SkyCoverId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseFtAgl = table.Column<int>(type: "INTEGER", nullable: true),
                    StationId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkyCoverObservation", x => x.SkyCoverId);
                    table.ForeignKey(
                        name: "FK_SkyCoverObservation_Metars_StationId",
                        column: x => x.StationId,
                        principalTable: "Metars",
                        principalColumn: "StationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "End",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TrueHeading = table.Column<int>(type: "INTEGER", nullable: true),
                    MagneticHeading = table.Column<int>(type: "INTEGER", nullable: false),
                    EndElevation = table.Column<double>(type: "REAL", nullable: true),
                    TdzElevation = table.Column<double>(type: "REAL", nullable: true),
                    RunwayName = table.Column<string>(type: "TEXT", nullable: false),
                    AirportFaaId = table.Column<string>(type: "TEXT", nullable: false),
                    RunwayOwnerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_End", x => x.Id);
                    table.ForeignKey(
                        name: "FK_End_Runway_RunwayOwnerId",
                        column: x => x.RunwayOwnerId,
                        principalTable: "Runway",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_End_RunwayOwnerId",
                table: "End",
                column: "RunwayOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Runway_AirportFaaId",
                table: "Runway",
                column: "AirportFaaId");

            migrationBuilder.CreateIndex(
                name: "IX_SkyCoverObservation_StationId",
                table: "SkyCoverObservation",
                column: "StationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AircraftTypes");

            migrationBuilder.DropTable(
                name: "Airlines");

            migrationBuilder.DropTable(
                name: "Artccs");

            migrationBuilder.DropTable(
                name: "Atises");

            migrationBuilder.DropTable(
                name: "CompletedJobs");

            migrationBuilder.DropTable(
                name: "End");

            migrationBuilder.DropTable(
                name: "RvrObservations");

            migrationBuilder.DropTable(
                name: "SkyCoverObservation");

            migrationBuilder.DropTable(
                name: "VatsimSnapshots");

            migrationBuilder.DropTable(
                name: "Runway");

            migrationBuilder.DropTable(
                name: "Metars");

            migrationBuilder.DropTable(
                name: "Airports");
        }
    }
}

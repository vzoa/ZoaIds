using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZoaIdsBackend.Modules.NasrData.Models;

namespace ZoaIdsBackend.Modules.NasrData.Data;

public class AirportConfiguration : IEntityTypeConfiguration<Airport>
{
    public void Configure(EntityTypeBuilder<Airport> builder)
    {
        builder
            .Ignore(a => a.RunwayEnds)
            .HasKey(a => a.FaaId);

        builder.OwnsOne(a => a.Location);

        builder.OwnsMany(
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
    }
}


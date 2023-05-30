using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZoaIdsBackend.Modules.Weather.Models;

namespace ZoaIdsBackend.Modules.Weather.Data;

public class MetarConfiguration : IEntityTypeConfiguration<Metar>
{
    public void Configure(EntityTypeBuilder<Metar> builder)
    {
        builder.HasKey(m => m.StationId);
        builder.OwnsOne(m => m.Location);
        builder.OwnsOne(m => m.Wind);
        builder.OwnsMany(
            m => m.SkyCovers, s =>
            {
                s.WithOwner().HasForeignKey("StationId");
                s.Property<int>("SkyCoverId");
                s.HasKey("SkyCoverId");
            });
    }
}


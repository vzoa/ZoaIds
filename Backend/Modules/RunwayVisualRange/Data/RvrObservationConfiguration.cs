using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZoaIdsBackend.Modules.RunwayVisualRange.Models;

namespace ZoaIdsBackend.Modules.RunwayVisualRange.Data;

public class RvrObservationConfiguration : IEntityTypeConfiguration<RvrObservation>
{
    public void Configure(EntityTypeBuilder<RvrObservation> builder)
    {
        builder.Property<int>("Id");
        builder.HasKey("Id");
    }
}

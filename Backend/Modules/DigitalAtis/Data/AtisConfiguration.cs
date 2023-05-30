using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZoaIdsBackend.Modules.DigitalAtis.Models;

namespace ZoaIdsBackend.Modules.DigitalAtis.Data;

public class AtisConfiguration : IEntityTypeConfiguration<Atis>
{
    public void Configure(EntityTypeBuilder<Atis> builder)
    {
        // Atis needs a composite primary key because a given airport can
        // have both a departure and an arrival ATIS
        builder
            .HasKey(a => new { a.IcaoId, a.Type });
    }
}


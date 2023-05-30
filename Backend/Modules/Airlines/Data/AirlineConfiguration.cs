using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZoaIdsBackend.Modules.Airlines.Models;

namespace ZoaIdsBackend.Modules.Airlines.Data;

public class AirlineConfiguration : IEntityTypeConfiguration<Airline>
{
    public void Configure(EntityTypeBuilder<Airline> builder)
    {
        builder.HasKey(a => a.IcaoId);
    }
}


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZoaIdsBackend.Modules.VatsimData.Models;

namespace ZoaIdsBackend.Modules.VatsimData.Data;

public class ArtccConfiguration : IEntityTypeConfiguration<Artcc>
{
    public void Configure(EntityTypeBuilder<Artcc> builder)
    {
        builder.HasKey(a => a.Guid);
    }
}

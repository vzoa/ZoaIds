using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZoaIdsBackend.Modules.AircraftTypes.Models;

namespace ZoaIdsBackend.Modules.AircraftTypes.Data;

public class AircraftTypeConfiguration : IEntityTypeConfiguration<AircraftType>
{
    public void Configure(EntityTypeBuilder<AircraftType> builder)
    {
        // Aircraft Type Info needs a shadow property primary key because
        // there can be multiple entries for a single aircraft ICAO code
        builder
            .Property<int>("AircraftTypeId")
            .ValueGeneratedOnAdd();
    }
}

using Ceataec.ExampleService.Domain.Voyages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Voyages;

public sealed class VoyageConfiguration : IEntityTypeConfiguration<Voyage>
{
    public void Configure(EntityTypeBuilder<Voyage> builder)
    {
        builder.ToTable("voyages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.VesselId).IsRequired();
        builder.Property(x => x.Destination).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.VesselId);
        // Cross-Aggregate: VesselId only — no HasOne<Vessel>() / SQL FK.
    }
}

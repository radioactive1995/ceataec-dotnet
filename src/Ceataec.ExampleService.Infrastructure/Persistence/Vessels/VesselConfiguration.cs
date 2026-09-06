using Ceataec.ExampleService.Domain.Vessels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Vessels;

public sealed class VesselConfiguration : IEntityTypeConfiguration<Vessel>
{
    public void Configure(EntityTypeBuilder<Vessel> builder)
    {
        builder.ToTable("vessels");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ImoNumber).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.ImoNumber).IsUnique();
        builder.HasMany(x => x.Tanks)
            .WithOne(x => x.Vessel)
            .HasForeignKey(x => x.VesselId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

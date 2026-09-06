using Ceataec.ExampleService.Domain.Vessels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Vessels;

public sealed class TankConfiguration : IEntityTypeConfiguration<Tank>
{
    public void Configure(EntityTypeBuilder<Tank> builder)
    {
        builder.ToTable("tanks");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CapacityCubicMeters).HasPrecision(18, 2);
        builder.HasOne(x => x.Vessel)
            .WithMany(x => x.Tanks)
            .HasForeignKey(x => x.VesselId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

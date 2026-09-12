using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Domain.Vessels.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Vessels;

public sealed class TankConfiguration : IEntityTypeConfiguration<Tank>
{
    public void Configure(EntityTypeBuilder<Tank> builder)
    {
        builder.ToTable("tanks");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql(PostgresSql.NewUuid);
        builder.Property(x => x.Name).HasMaxLength(Tank.NameMaxLength).IsRequired();
        builder.Property(x => x.CapacityCubicMeters).HasPrecision(18, 2);
        builder.HasOne(x => x.Vessel)
            .WithMany(x => x.Tanks)
            .HasForeignKey(x => x.VesselId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

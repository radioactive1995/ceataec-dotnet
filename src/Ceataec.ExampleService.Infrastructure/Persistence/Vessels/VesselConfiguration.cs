using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Domain.Vessels.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Vessels;

public sealed class VesselConfiguration : IEntityTypeConfiguration<Vessel>
{
    public void Configure(EntityTypeBuilder<Vessel> builder)
    {
        builder.ToTable("vessels");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql(PostgresSql.NewUuid);
        builder.Property(x => x.Name).HasMaxLength(Vessel.NameMaxLength).IsRequired();
        builder.Property(x => x.ImoNumber)
            .HasConversion(
                imo => imo.Value,
                value => ImoNumber.FromPersistence(value))
            .HasMaxLength(ImoNumber.MaxLength)
            .IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(Vessel.CreatedByMaxLength).IsRequired();
        builder.HasIndex(x => x.ImoNumber).IsUnique();
        builder.HasMany(x => x.Tanks)
            .WithOne(x => x.Vessel)
            .HasForeignKey(x => x.VesselId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Ceataec.ExampleService.Domain.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Certificates;

public sealed class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("certificates");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasDefaultValueSql(PostgresSql.NewUuid);
        builder.Property(x => x.VesselId).IsRequired();
        builder.Property(x => x.Type).HasMaxLength(Certificate.TypeMaxLength).IsRequired();
        builder.HasIndex(x => x.VesselId);
        // Cross-module: VesselId only — no HasOne<Vessel>() / SQL FK.
    }
}

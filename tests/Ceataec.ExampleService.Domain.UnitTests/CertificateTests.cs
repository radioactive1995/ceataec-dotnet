using Ceataec.ExampleService.Domain;
using Ceataec.ExampleService.Domain.Certificates;

namespace Ceataec.ExampleService.Domain.UnitTests;

public sealed class CertificateTests
{
    [Fact]
    public void Create_normalizes_type()
    {
        var vesselId = Guid.NewGuid();
        var issuedOn = DateTimeOffset.UtcNow;

        var certificate = Certificate.Create(vesselId, "  Safety  ", issuedOn);

        Assert.Equal(Guid.Empty, certificate.Id);
        Assert.Equal(vesselId, certificate.VesselId);
        Assert.Equal("Safety", certificate.Type);
        Assert.Equal(issuedOn, certificate.IssuedOn);
    }

    [Fact]
    public void Create_rejects_missing_vessel()
    {
        var ex = Assert.Throws<DomainException>(
            () => Certificate.Create(Guid.Empty, "Safety", DateTimeOffset.UtcNow));

        Assert.Equal("vessel_id_required", ex.Code);
    }

    [Fact]
    public void Create_rejects_missing_type()
    {
        var ex = Assert.Throws<DomainException>(
            () => Certificate.Create(Guid.NewGuid(), " ", DateTimeOffset.UtcNow));

        Assert.Equal("certificate_type_required", ex.Code);
    }
}

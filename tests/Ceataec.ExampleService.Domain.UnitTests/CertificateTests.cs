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

        Assert.False(certificate.IsError);
        Assert.Equal(Guid.Empty, certificate.Value.Id);
        Assert.Equal(vesselId, certificate.Value.VesselId);
        Assert.Equal("Safety", certificate.Value.Type);
        Assert.Equal(issuedOn, certificate.Value.IssuedOn);
    }

    [Fact]
    public void Create_rejects_missing_vessel()
    {
        var result = Certificate.Create(Guid.Empty, "Safety", DateTimeOffset.UtcNow);

        Assert.True(result.IsError);
        Assert.Equal("vessel_id_required", result.FirstError.Code);
    }

    [Fact]
    public void Create_rejects_missing_type()
    {
        var result = Certificate.Create(Guid.NewGuid(), " ", DateTimeOffset.UtcNow);

        Assert.True(result.IsError);
        Assert.Equal("certificate_type_required", result.FirstError.Code);
    }
}

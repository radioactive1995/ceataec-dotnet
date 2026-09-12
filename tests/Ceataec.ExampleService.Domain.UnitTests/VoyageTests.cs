using Ceataec.ExampleService.Domain;
using Ceataec.ExampleService.Domain.Voyages;

namespace Ceataec.ExampleService.Domain.UnitTests;

public sealed class VoyageTests
{
    [Fact]
    public void Create_sets_identity_and_normalizes_destination()
    {
        var vesselId = Guid.NewGuid();
        var departureAt = DateTimeOffset.UtcNow.AddDays(1);

        var voyage = Voyage.Create(vesselId, "  Rotterdam  ", departureAt);

        Assert.NotEqual(Guid.Empty, voyage.Id);
        Assert.Equal(vesselId, voyage.VesselId);
        Assert.Equal("Rotterdam", voyage.Destination);
        Assert.Equal(departureAt, voyage.DepartureAt);
    }

    [Fact]
    public void Create_rejects_missing_vessel()
    {
        var ex = Assert.Throws<DomainException>(
            () => Voyage.Create(Guid.Empty, "Rotterdam", DateTimeOffset.UtcNow));

        Assert.Equal("vessel_id_required", ex.Code);
    }

    [Fact]
    public void Create_rejects_missing_destination()
    {
        var ex = Assert.Throws<DomainException>(
            () => Voyage.Create(Guid.NewGuid(), " ", DateTimeOffset.UtcNow));

        Assert.Equal("voyage_destination_required", ex.Code);
    }
}

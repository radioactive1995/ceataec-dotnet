using Ceataec.ExampleService.Domain.Voyages;

namespace Ceataec.ExampleService.Domain.UnitTests;

public sealed class VoyageTests
{
    [Fact]
    public void Create_normalizes_destination()
    {
        var vesselId = Guid.NewGuid();
        var departureAt = DateTimeOffset.UtcNow.AddDays(1);

        var voyage = Voyage.Create(vesselId, "  Rotterdam  ", departureAt);

        Assert.False(voyage.IsError);
        Assert.Equal(Guid.Empty, voyage.Value.Id);
        Assert.Equal(vesselId, voyage.Value.VesselId);
        Assert.Equal("Rotterdam", voyage.Value.Destination);
        Assert.Equal(departureAt, voyage.Value.DepartureAt);
    }

    [Fact]
    public void Create_rejects_missing_vessel()
    {
        var result = Voyage.Create(Guid.Empty, "Rotterdam", DateTimeOffset.UtcNow);

        Assert.True(result.IsError);
        Assert.Equal("vessel_id_required", result.FirstError.Code);
    }

    [Fact]
    public void Create_rejects_missing_destination()
    {
        var result = Voyage.Create(Guid.NewGuid(), " ", DateTimeOffset.UtcNow);

        Assert.True(result.IsError);
        Assert.Equal("voyage_destination_required", result.FirstError.Code);
    }
}

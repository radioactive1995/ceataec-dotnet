using Ceataec.ExampleService.Domain;
using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Domain.Vessels.ValueObjects;

namespace Ceataec.ExampleService.Domain.UnitTests.Vessels;

public sealed class VesselTests
{
    [Fact]
    public void Create_sets_identity_and_trimmed_fields()
    {
        var imo = ImoNumber.Create("IMO9999999");

        var vessel = Vessel.Create("  Aurora  ", imo, "  user-1  ");

        Assert.NotEqual(Guid.Empty, vessel.Id);
        Assert.Equal("Aurora", vessel.Name);
        Assert.Equal(imo, vessel.ImoNumber);
        Assert.Equal("user-1", vessel.CreatedBy);
        Assert.Empty(vessel.Tanks);
        Assert.True(vessel.CreatedAt <= DateTimeOffset.UtcNow);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_missing_name(string? name)
    {
        var imo = ImoNumber.Create("IMO9999999");

        var ex = Assert.Throws<DomainException>(() => Vessel.Create(name!, imo, "user-1"));

        Assert.Equal("vessel_name_required", ex.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_missing_created_by(string? createdBy)
    {
        var imo = ImoNumber.Create("IMO9999999");

        var ex = Assert.Throws<DomainException>(() => Vessel.Create("Aurora", imo, createdBy!));

        Assert.Equal("created_by_required", ex.Code);
    }

    [Fact]
    public void Create_rejects_name_over_maximum_length()
    {
        var imo = ImoNumber.Create("IMO9999999");

        var ex = Assert.Throws<DomainException>(
            () => Vessel.Create(new string('a', Vessel.NameMaxLength + 1), imo, "user-1"));

        Assert.Equal("vessel_name_too_long", ex.Code);
    }

    [Fact]
    public void AddTank_adds_trimmed_tank_owned_by_vessel()
    {
        var vessel = Vessel.Create("Aurora", ImoNumber.Create("IMO9999999"), "user-1");

        vessel.AddTank("  Cargo 1  ", 1250.50m);

        var tank = Assert.Single(vessel.Tanks);
        Assert.NotEqual(Guid.Empty, tank.Id);
        Assert.Equal(vessel.Id, tank.VesselId);
        Assert.Same(vessel, tank.Vessel);
        Assert.Equal("Cargo 1", tank.Name);
        Assert.Equal(1250.50m, tank.CapacityCubicMeters);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddTank_rejects_missing_name(string? name)
    {
        var vessel = Vessel.Create("Aurora", ImoNumber.Create("IMO9999999"), "user-1");

        var ex = Assert.Throws<DomainException>(() => vessel.AddTank(name!, 100));

        Assert.Equal("tank_name_required", ex.Code);
        Assert.Empty(vessel.Tanks);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddTank_rejects_non_positive_capacity(decimal capacity)
    {
        var vessel = Vessel.Create("Aurora", ImoNumber.Create("IMO9999999"), "user-1");

        var ex = Assert.Throws<DomainException>(() => vessel.AddTank("Cargo 1", capacity));

        Assert.Equal("tank_capacity_invalid", ex.Code);
        Assert.Empty(vessel.Tanks);
    }
}

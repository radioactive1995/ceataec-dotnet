using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Domain.Vessels.ValueObjects;

namespace Ceataec.ExampleService.Domain.UnitTests.Vessels;

public sealed class VesselTests
{
    [Fact]
    public void Create_trims_fields()
    {
        var imo = ImoNumber.Create("IMO9999999").Value;

        var vessel = Vessel.Create("  Aurora  ", imo, "  user-1  ");

        Assert.False(vessel.IsError);
        Assert.Equal(Guid.Empty, vessel.Value.Id);
        Assert.Equal("Aurora", vessel.Value.Name);
        Assert.Equal(imo, vessel.Value.ImoNumber);
        Assert.Equal("user-1", vessel.Value.CreatedBy);
        Assert.Empty(vessel.Value.Tanks);
        Assert.True(vessel.Value.CreatedAt <= DateTimeOffset.UtcNow);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_missing_name(string? name)
    {
        var imo = ImoNumber.Create("IMO9999999").Value;

        var result = Vessel.Create(name!, imo, "user-1");

        Assert.True(result.IsError);
        Assert.Equal("vessel_name_required", result.FirstError.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_missing_created_by(string? createdBy)
    {
        var imo = ImoNumber.Create("IMO9999999").Value;

        var result = Vessel.Create("Aurora", imo, createdBy!);

        Assert.True(result.IsError);
        Assert.Equal("created_by_required", result.FirstError.Code);
    }

    [Fact]
    public void Create_rejects_name_over_maximum_length()
    {
        var imo = ImoNumber.Create("IMO9999999").Value;

        var result = Vessel.Create(new string('a', Vessel.NameMaxLength + 1), imo, "user-1");

        Assert.True(result.IsError);
        Assert.Equal("vessel_name_too_long", result.FirstError.Code);
    }

    [Fact]
    public void AddTank_adds_trimmed_tank_owned_by_vessel()
    {
        var vessel = Vessel.Create("Aurora", ImoNumber.Create("IMO9999999").Value, "user-1").Value;

        var added = vessel.AddTank("  Cargo 1  ", 1250.50m);

        Assert.False(added.IsError);
        var tank = Assert.Single(vessel.Tanks);
        Assert.Equal(Guid.Empty, tank.Id);
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
        var vessel = Vessel.Create("Aurora", ImoNumber.Create("IMO9999999").Value, "user-1").Value;

        var result = vessel.AddTank(name!, 100);

        Assert.True(result.IsError);
        Assert.Equal("tank_name_required", result.FirstError.Code);
        Assert.Empty(vessel.Tanks);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddTank_rejects_non_positive_capacity(decimal capacity)
    {
        var vessel = Vessel.Create("Aurora", ImoNumber.Create("IMO9999999").Value, "user-1").Value;

        var result = vessel.AddTank("Cargo 1", capacity);

        Assert.True(result.IsError);
        Assert.Equal("tank_capacity_invalid", result.FirstError.Code);
        Assert.Empty(vessel.Tanks);
    }
}

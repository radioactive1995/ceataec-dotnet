using Ceataec.ExampleService.Domain.Vessels.ValueObjects;

namespace Ceataec.ExampleService.Domain.UnitTests.Vessels;

public sealed class ImoNumberTests
{
    [Fact]
    public void Create_trims_and_accepts_valid_value()
    {
        var imo = ImoNumber.Create("  IMO1234567  ");

        Assert.False(imo.IsError);
        Assert.Equal("IMO1234567", imo.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_rejects_missing_value(string? value)
    {
        var result = ImoNumber.Create(value!);

        Assert.True(result.IsError);
        Assert.Equal("imo_number_required", result.FirstError.Code);
    }

    [Fact]
    public void Create_rejects_value_longer_than_max()
    {
        var value = new string('X', ImoNumber.MaxLength + 1);

        var result = ImoNumber.Create(value);

        Assert.True(result.IsError);
        Assert.Equal("imo_number_too_long", result.FirstError.Code);
    }

    [Fact]
    public void FromPersistence_does_not_revalidate()
    {
        var imo = ImoNumber.FromPersistence("stored-as-is");

        Assert.Equal("stored-as-is", imo.Value);
    }
}

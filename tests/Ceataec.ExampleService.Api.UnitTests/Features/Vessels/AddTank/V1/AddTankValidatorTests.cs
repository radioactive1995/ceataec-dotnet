using Ceataec.ExampleService.Domain.Vessels.Entities;
using Ceataec.ExampleService.Features.Vessels.AddTank.V1;
using FluentValidation.TestHelper;

namespace Ceataec.ExampleService.Api.UnitTests.Features.Vessels.AddTank.V1;

public sealed class AddTankValidatorTests
{
    private readonly AddTankValidator _sut = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _sut.TestValidate(new AddTankRequest("Cargo 1", 1250.50m));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_name_fails()
    {
        var result = _sut.TestValidate(new AddTankRequest("", 100m));

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_over_maximum_length_fails()
    {
        var result = _sut.TestValidate(new AddTankRequest(new string('a', Tank.NameMaxLength + 1), 100m));

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Non_positive_capacity_fails(decimal capacity)
    {
        var result = _sut.TestValidate(new AddTankRequest("Cargo 1", capacity));

        result.ShouldHaveValidationErrorFor(x => x.CapacityCubicMeters);
    }
}

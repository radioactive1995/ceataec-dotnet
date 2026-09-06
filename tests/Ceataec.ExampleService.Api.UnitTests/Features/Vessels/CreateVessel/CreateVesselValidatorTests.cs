using Ceataec.ExampleService.Features.Vessels.CreateVessel;
using FluentValidation.TestHelper;

namespace Ceataec.ExampleService.Api.UnitTests.Features.Vessels.CreateVessel;

public sealed class CreateVesselValidatorTests
{
    private readonly CreateVesselValidator _sut = new();

    [Fact]
    public void Valid_request_passes()
    {
        var result = _sut.TestValidate(new CreateVesselRequest("Pacific Explorer", "IMO9123456"));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_name_fails()
    {
        var result = _sut.TestValidate(new CreateVesselRequest("", "IMO9123456"));

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Empty_imo_fails()
    {
        var result = _sut.TestValidate(new CreateVesselRequest("Pacific Explorer", ""));

        result.ShouldHaveValidationErrorFor(x => x.ImoNumber);
    }
}

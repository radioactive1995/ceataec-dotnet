using FastEndpoints;
using FluentValidation;

namespace Ceataec.ExampleService.Features.Vessels.CreateVessel;

public sealed class CreateVesselValidator : Validator<CreateVesselRequest>
{
    public CreateVesselValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ImoNumber).NotEmpty().MaximumLength(20);
    }
}

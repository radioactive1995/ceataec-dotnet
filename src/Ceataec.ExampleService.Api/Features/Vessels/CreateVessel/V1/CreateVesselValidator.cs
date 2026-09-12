using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Domain.Vessels.ValueObjects;
using FastEndpoints;
using FluentValidation;

namespace Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;

public sealed class CreateVesselValidator : Validator<CreateVesselRequest>
{
    public CreateVesselValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Vessel.NameMaxLength);
        RuleFor(x => x.ImoNumber).NotEmpty().MaximumLength(ImoNumber.MaxLength);
    }
}

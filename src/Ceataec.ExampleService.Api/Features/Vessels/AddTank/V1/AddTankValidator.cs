using Ceataec.ExampleService.Domain.Vessels.Entities;
using FastEndpoints;
using FluentValidation;

namespace Ceataec.ExampleService.Features.Vessels.AddTank.V1;

public sealed class AddTankValidator : Validator<AddTankRequest>
{
    public AddTankValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Tank.NameMaxLength);
        RuleFor(x => x.CapacityCubicMeters).GreaterThan(0);
    }
}

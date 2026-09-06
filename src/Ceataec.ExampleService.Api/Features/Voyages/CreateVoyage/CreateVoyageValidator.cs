using FastEndpoints;
using FluentValidation;

namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage;

public sealed class CreateVoyageValidator : Validator<CreateVoyageRequest>
{
    public CreateVoyageValidator()
    {
        RuleFor(x => x.VesselId).NotEmpty();
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DepartureAt).NotEmpty();
    }
}

using Ceataec.ExampleService.Domain.Voyages;
using FastEndpoints;
using FluentValidation;

namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage.V1;

public sealed class CreateVoyageValidator : Validator<CreateVoyageRequest>
{
    public CreateVoyageValidator()
    {
        RuleFor(x => x.VesselId).NotEmpty();
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(Voyage.DestinationMaxLength);
        RuleFor(x => x.DepartureAt).NotEmpty();
    }
}

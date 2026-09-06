using FastEndpoints;

namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage;

public sealed record CreateVoyageCommand(
    Guid VesselId,
    string Destination,
    DateTimeOffset DepartureAt) : ICommand<CreateVoyageResponse?>;

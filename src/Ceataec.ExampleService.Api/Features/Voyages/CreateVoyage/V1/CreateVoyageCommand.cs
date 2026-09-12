using FastEndpoints;

namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage.V1;

public sealed record CreateVoyageCommand(
    Guid VesselId,
    string Destination,
    DateTimeOffset DepartureAt) : ICommand<CreateVoyageResponse?>;

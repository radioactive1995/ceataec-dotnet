namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage.V1;

public sealed record CreateVoyageResponse(
    Guid Id,
    Guid VesselId,
    string Destination,
    DateTimeOffset DepartureAt);

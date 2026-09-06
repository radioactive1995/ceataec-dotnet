namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage;

public sealed record CreateVoyageResponse(
    Guid Id,
    Guid VesselId,
    string Destination,
    DateTimeOffset DepartureAt);

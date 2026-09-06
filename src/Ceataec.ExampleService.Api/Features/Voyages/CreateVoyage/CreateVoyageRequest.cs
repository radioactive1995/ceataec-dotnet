namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage;

public sealed record CreateVoyageRequest(
    Guid VesselId,
    string Destination,
    DateTimeOffset DepartureAt);

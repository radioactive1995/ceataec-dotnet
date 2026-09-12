namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage.V1;

public sealed record CreateVoyageRequest(
    Guid VesselId,
    string Destination,
    DateTimeOffset DepartureAt);

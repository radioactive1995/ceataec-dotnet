namespace Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;

public sealed record CreateVesselResponse(
    Guid Id,
    string Name,
    string ImoNumber,
    string CreatedBy);

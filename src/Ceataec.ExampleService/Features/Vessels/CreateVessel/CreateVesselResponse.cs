namespace Ceataec.ExampleService.Features.Vessels.CreateVessel;

public sealed record CreateVesselResponse(
    Guid Id,
    string Name,
    string ImoNumber,
    string CreatedBy);

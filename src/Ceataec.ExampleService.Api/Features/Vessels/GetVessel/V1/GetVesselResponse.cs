namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V1;

public sealed record GetVesselResponse(
    Guid Id,
    string Name,
    string ImoNumber,
    string CreatedBy,
    IReadOnlyList<TankDto> Tanks);

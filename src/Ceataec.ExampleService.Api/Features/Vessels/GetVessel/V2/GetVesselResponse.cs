namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V2;

public sealed record GetVesselResponse(
    Guid Id,
    string Name,
    string ImoNumber,
    int TankCount,
    decimal TotalCapacityCubicMeters);

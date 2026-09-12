namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V1;

public sealed record TankDto(
    Guid Id,
    string Name,
    decimal CapacityCubicMeters);

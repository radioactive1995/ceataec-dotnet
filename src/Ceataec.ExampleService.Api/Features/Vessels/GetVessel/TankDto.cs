namespace Ceataec.ExampleService.Features.Vessels.GetVessel;

public sealed record TankDto(
    Guid Id,
    string Name,
    decimal CapacityCubicMeters);

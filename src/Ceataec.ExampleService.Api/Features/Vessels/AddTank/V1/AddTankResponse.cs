namespace Ceataec.ExampleService.Features.Vessels.AddTank.V1;

public sealed record AddTankResponse(
    Guid Id,
    Guid VesselId,
    string Name,
    decimal CapacityCubicMeters);

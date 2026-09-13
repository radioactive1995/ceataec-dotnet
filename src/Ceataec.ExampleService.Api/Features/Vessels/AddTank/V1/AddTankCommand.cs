using ErrorOr;
using FastEndpoints;

namespace Ceataec.ExampleService.Features.Vessels.AddTank.V1;

public sealed record AddTankCommand(
    Guid VesselId,
    string Name,
    decimal CapacityCubicMeters) : ICommand<ErrorOr<AddTankResponse>>;

using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Infrastructure.Persistence;
using ErrorOr;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Ceataec.ExampleService.Features.Vessels.AddTank.V1;

public sealed class AddTankHandler(ICommandDbContext dbContext, ILogger<AddTankHandler> logger)
    : ICommandHandler<AddTankCommand, ErrorOr<AddTankResponse>>
{
    public async Task<ErrorOr<AddTankResponse>> ExecuteAsync(
        AddTankCommand command,
        CancellationToken ct)
    {
        // Load tanks so EF tracks the existing collection; adding to an unloaded
        // collection can persist as a replace and delete sibling tanks.
        var vessel = await dbContext.Set<Vessel>()
            .Include(v => v.Tanks)
            .FirstOrDefaultAsync(v => v.Id == command.VesselId, ct);

        if (vessel is null)
        {
            logger.LogWarning("Rejected tank for unknown vessel {VesselId}", command.VesselId);
            return Error.NotFound("vessel_not_found", "Vessel was not found.");
        }

        var tank = vessel.AddTank(command.Name, command.CapacityCubicMeters);
        if (tank.IsError)
        {
            return tank.Errors;
        }

        await dbContext.SaveChangesAsync(ct);

        return new AddTankResponse(
            tank.Value.Id,
            tank.Value.VesselId,
            tank.Value.Name,
            tank.Value.CapacityCubicMeters);
    }
}

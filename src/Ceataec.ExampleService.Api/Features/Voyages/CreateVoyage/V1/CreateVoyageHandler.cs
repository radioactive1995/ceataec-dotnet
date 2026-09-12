using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Domain.Voyages;
using Ceataec.ExampleService.Infrastructure.Persistence;
using ErrorOr;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage.V1;

public sealed class CreateVoyageHandler(
    ICommandDbContext dbContext,
    ILogger<CreateVoyageHandler> logger)
    : ICommandHandler<CreateVoyageCommand, ErrorOr<CreateVoyageResponse>>
{
    public async Task<ErrorOr<CreateVoyageResponse>> ExecuteAsync(
        CreateVoyageCommand command,
        CancellationToken ct)
    {
        var vesselExists = await dbContext.Set<Vessel>()
            .AnyAsync(v => v.Id == command.VesselId, ct);

        if (!vesselExists)
        {
            logger.LogWarning("Rejected voyage for unknown vessel {VesselId}", command.VesselId);
            return Error.NotFound("vessel_not_found", "Vessel was not found.");
        }

        var voyage = Voyage.Create(
            command.VesselId,
            command.Destination,
            command.DepartureAt);

        if (voyage.IsError)
        {
            return voyage.Errors;
        }

        dbContext.Set<Voyage>().Add(voyage.Value);
        await dbContext.SaveChangesAsync(ct);

        return new CreateVoyageResponse(
            voyage.Value.Id,
            voyage.Value.VesselId,
            voyage.Value.Destination,
            voyage.Value.DepartureAt);
    }
}

using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Domain.Vessels.ValueObjects;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Infrastructure.Providers;
using ErrorOr;
using FastEndpoints;

namespace Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;

public sealed class CreateVesselHandler(
    ICommandDbContext dbContext,
    IUserProvider userProvider,
    ILogger<CreateVesselHandler> logger)
    : ICommandHandler<CreateVesselCommand, ErrorOr<CreateVesselResponse>>
{
    public async Task<ErrorOr<CreateVesselResponse>> ExecuteAsync(
        CreateVesselCommand command,
        CancellationToken ct)
    {
        var createdBy = userProvider.GetCurrentUserId();

        var vessel = ImoNumber.Create(command.ImoNumber)
            .Then(imo => Vessel.Create(command.Name, imo, createdBy));

        if (vessel.IsError)
        {
            return vessel.Errors;
        }

        dbContext.Set<Vessel>().Add(vessel.Value);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation(
            "Created vessel {VesselId} ({ImoNumber}) by {CreatedBy}",
            vessel.Value.Id,
            vessel.Value.ImoNumber.Value,
            createdBy);

        return new CreateVesselResponse(
            vessel.Value.Id,
            vessel.Value.Name,
            vessel.Value.ImoNumber.Value,
            vessel.Value.CreatedBy);
    }
}

using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Domain.Vessels.ValueObjects;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Infrastructure.Providers;
using FastEndpoints;

namespace Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;

public sealed class CreateVesselHandler(
    ICommandDbContext dbContext,
    IUserProvider userProvider,
    ILogger<CreateVesselHandler> logger)
    : ICommandHandler<CreateVesselCommand, CreateVesselResponse>
{
    public async Task<CreateVesselResponse> ExecuteAsync(
        CreateVesselCommand command,
        CancellationToken ct)
    {
        var createdBy = userProvider.GetCurrentUserId();
        var vessel = Vessel.Create(
            command.Name,
            ImoNumber.Create(command.ImoNumber),
            createdBy);

        dbContext.Set<Vessel>().Add(vessel);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation(
            "Created vessel {VesselId} ({ImoNumber}) by {CreatedBy}",
            vessel.Id,
            vessel.ImoNumber.Value,
            createdBy);

        return new CreateVesselResponse(
            vessel.Id,
            vessel.Name,
            vessel.ImoNumber.Value,
            vessel.CreatedBy);
    }
}

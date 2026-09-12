using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Domain.Vessels.ValueObjects;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Infrastructure.Providers;
using FastEndpoints;

namespace Ceataec.ExampleService.Features.Vessels.CreateVessel;

public sealed class CreateVesselHandler(
    AppDbContext dbContext,
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

        dbContext.Vessels.Add(vessel);
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

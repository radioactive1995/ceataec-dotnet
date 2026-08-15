using Ceataec.ExampleService.Infrastructure.Providers;
using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Infrastructure.Persistence.Vessels;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Vessels.CreateVessel;

public sealed class CreateVesselEndpoint(
    IVesselRepository vessels,
    IUserProvider userProvider,
    ILogger<CreateVesselEndpoint> logger)
    : Endpoint<CreateVesselRequest, Created<CreateVesselResponse>>
{
    public override void Configure()
    {
        Post("/vessels");
        AllowAnonymous();
    }

    public override async Task<Created<CreateVesselResponse>> ExecuteAsync(
        CreateVesselRequest req,
        CancellationToken ct)
    {
        var createdBy = userProvider.GetCurrentUserId();

        var vessel = new Vessel
        {
            Id = Guid.NewGuid(),
            Name = req.Name.Trim(),
            ImoNumber = req.ImoNumber.Trim(),
            CreatedBy = createdBy,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await vessels.AddAsync(vessel, ct);

        logger.LogInformation(
            "Created vessel {VesselId} ({ImoNumber}) by {CreatedBy}",
            vessel.Id,
            vessel.ImoNumber,
            createdBy);

        var response = new CreateVesselResponse(
            vessel.Id,
            vessel.Name,
            vessel.ImoNumber,
            vessel.CreatedBy);

        return TypedResults.Created($"/vessels/{vessel.Id}", response);
    }
}

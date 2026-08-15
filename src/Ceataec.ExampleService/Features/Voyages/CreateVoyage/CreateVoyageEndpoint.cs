using Ceataec.ExampleService.Infrastructure.Persistence.Vessels;
using Ceataec.ExampleService.Domain.Voyages;
using Ceataec.ExampleService.Infrastructure.Persistence.Voyages;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage;

public sealed class CreateVoyageEndpoint(
    IVoyageRepository voyages,
    IVesselRepository vessels,
    ILogger<CreateVoyageEndpoint> logger)
    : Endpoint<CreateVoyageRequest, Results<Created<CreateVoyageResponse>, NotFound>>
{
    public override void Configure()
    {
        Post("/voyages");
        AllowAnonymous();
    }

    public override async Task<Results<Created<CreateVoyageResponse>, NotFound>> ExecuteAsync(
        CreateVoyageRequest req,
        CancellationToken ct)
    {
        if (!await vessels.ExistsAsync(req.VesselId, ct))
        {
            logger.LogWarning("Rejected voyage for unknown vessel {VesselId}", req.VesselId);
            return TypedResults.NotFound();
        }

        var voyage = new Voyage
        {
            Id = Guid.NewGuid(),
            VesselId = req.VesselId,
            Destination = req.Destination.Trim(),
            DepartureAt = req.DepartureAt
        };

        await voyages.AddAsync(voyage, ct);

        var response = new CreateVoyageResponse(
            voyage.Id,
            voyage.VesselId,
            voyage.Destination,
            voyage.DepartureAt);

        return TypedResults.Created($"/voyages/{voyage.Id}", response);
    }
}

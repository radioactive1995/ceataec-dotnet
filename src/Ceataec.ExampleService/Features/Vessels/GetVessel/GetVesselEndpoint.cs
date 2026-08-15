using Ceataec.ExampleService.Infrastructure.Persistence.Vessels;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel;

public sealed class GetVesselEndpoint(IVesselRepository vessels)
    : EndpointWithoutRequest<Results<Ok<GetVesselResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("/vessels/{id:guid}");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<GetVesselResponse>, NotFound>> ExecuteAsync(
        CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var vessel = await vessels.GetByIdAsync(id, ct);

        if (vessel is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(
            new GetVesselResponse(
                vessel.Id,
                vessel.Name,
                vessel.ImoNumber,
                vessel.CreatedBy,
                vessel.Tanks
                    .Select(t => new TankDto(t.Id, t.Name, t.CapacityCubicMeters))
                    .ToList()));
    }
}

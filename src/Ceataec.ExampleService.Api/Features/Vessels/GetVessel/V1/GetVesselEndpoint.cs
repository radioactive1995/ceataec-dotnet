using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V1;

public sealed class GetVesselEndpoint
    : EndpointWithoutRequest<Results<Ok<GetVesselResponse>, NotFound>>
{
    public override void Configure()
    {
        Get("/vessels/{id:guid}");
        Version(1);
        AllowAnonymous();
    }

    public override async Task<Results<Ok<GetVesselResponse>, NotFound>> ExecuteAsync(
        CancellationToken ct)
    {
        var response = await new GetVesselQuery(Route<Guid>("id"))
            .ExecuteAsync(ct);

        if (response is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(response);
    }
}

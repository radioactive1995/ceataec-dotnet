using Ceataec.ExampleService.Http;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V1;

public sealed class GetVesselEndpoint
    : EndpointWithoutRequest<Results<Ok<GetVesselResponse>, ProblemHttpResult>>
{
    public override void Configure()
    {
        Get("/vessels/{id:guid}");
        Version(1);
        AllowAnonymous();
    }

    public override async Task<Results<Ok<GetVesselResponse>, ProblemHttpResult>> ExecuteAsync(
        CancellationToken ct)
    {
        var result = await new GetVesselQuery(Route<Guid>("id"))
            .ExecuteAsync(ct);

        return result.Match<Results<Ok<GetVesselResponse>, ProblemHttpResult>>(
            response => TypedResults.Ok(response),
            errors => errors.ToProblem());
    }
}

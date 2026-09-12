using Ceataec.ExampleService.Http;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage.V1;

public sealed class CreateVoyageEndpoint
    : Endpoint<CreateVoyageRequest, Results<Created<CreateVoyageResponse>, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/voyages");
        Version(1);
        AllowAnonymous();
    }

    public override async Task<Results<Created<CreateVoyageResponse>, ProblemHttpResult>> ExecuteAsync(
        CreateVoyageRequest req,
        CancellationToken ct)
    {
        var result = await new CreateVoyageCommand(
                req.VesselId,
                req.Destination,
                req.DepartureAt)
            .ExecuteAsync(ct);

        return result.Match<Results<Created<CreateVoyageResponse>, ProblemHttpResult>>(
            response => TypedResults.Created($"/v1/voyages/{response.Id}", response),
            errors => errors.ToProblem());
    }
}

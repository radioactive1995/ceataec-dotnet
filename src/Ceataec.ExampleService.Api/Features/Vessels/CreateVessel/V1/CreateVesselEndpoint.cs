using Ceataec.ExampleService.Http;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;

public sealed class CreateVesselEndpoint
    : Endpoint<CreateVesselRequest, Results<Created<CreateVesselResponse>, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/vessels");
        Version(1);
        AllowAnonymous();
    }

    public override async Task<Results<Created<CreateVesselResponse>, ProblemHttpResult>> ExecuteAsync(
        CreateVesselRequest req,
        CancellationToken ct)
    {
        var result = await new CreateVesselCommand(req.Name, req.ImoNumber)
            .ExecuteAsync(ct);

        return result.Match<Results<Created<CreateVesselResponse>, ProblemHttpResult>>(
            response => TypedResults.Created($"/v1/vessels/{response.Id}", response),
            errors => errors.ToProblem());
    }
}

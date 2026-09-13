using Ceataec.ExampleService.Http;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Vessels.AddTank.V1;

public sealed class AddTankEndpoint
    : Endpoint<AddTankRequest, Results<Created<AddTankResponse>, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/vessels/{id:guid}/tanks");
        Version(1);
        AllowAnonymous();
    }

    public override async Task<Results<Created<AddTankResponse>, ProblemHttpResult>> ExecuteAsync(
        AddTankRequest req,
        CancellationToken ct)
    {
        var result = await new AddTankCommand(
                Route<Guid>("id"),
                req.Name,
                req.CapacityCubicMeters)
            .ExecuteAsync(ct);

        return result.Match<Results<Created<AddTankResponse>, ProblemHttpResult>>(
            response => TypedResults.Created(
                $"/v1/vessels/{response.VesselId}/tanks/{response.Id}",
                response),
            errors => errors.ToProblem());
    }
}

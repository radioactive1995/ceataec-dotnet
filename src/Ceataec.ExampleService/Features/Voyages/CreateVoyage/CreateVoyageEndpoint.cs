using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage;

public sealed class CreateVoyageEndpoint
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
        var response = await new CreateVoyageCommand(
                req.VesselId,
                req.Destination,
                req.DepartureAt)
            .ExecuteAsync(ct);

        if (response is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Created($"/voyages/{response.Id}", response);
    }
}

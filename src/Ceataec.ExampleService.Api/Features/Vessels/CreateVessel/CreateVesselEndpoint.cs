using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Vessels.CreateVessel;

public sealed class CreateVesselEndpoint
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
        var response = await new CreateVesselCommand(req.Name, req.ImoNumber)
            .ExecuteAsync(ct);

        return TypedResults.Created($"/vessels/{response.Id}", response);
    }
}

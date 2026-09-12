using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;

public sealed class CreateVesselEndpoint
    : Endpoint<CreateVesselRequest, Created<CreateVesselResponse>>
{
    public override void Configure()
    {
        Post("/vessels");
        Version(1);
        AllowAnonymous();
    }

    public override async Task<Created<CreateVesselResponse>> ExecuteAsync(
        CreateVesselRequest req,
        CancellationToken ct)
    {
        var response = await new CreateVesselCommand(req.Name, req.ImoNumber)
            .ExecuteAsync(ct);

        return TypedResults.Created($"/v1/vessels/{response.Id}", response);
    }
}

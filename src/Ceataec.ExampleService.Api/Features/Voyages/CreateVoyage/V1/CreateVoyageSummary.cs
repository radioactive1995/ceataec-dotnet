using FastEndpoints;

namespace Ceataec.ExampleService.Features.Voyages.CreateVoyage.V1;

public sealed class CreateVoyageSummary : Summary<CreateVoyageEndpoint>
{
    public CreateVoyageSummary()
    {
        Summary = "Create a voyage";
        Description = "Schedules a voyage for an existing vessel. Stores VesselId only — no cross-module SQL FK.";
        ExampleRequest = new CreateVoyageRequest(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Rotterdam",
            DateTimeOffset.UtcNow.AddDays(3));
        Response<CreateVoyageResponse>(201, "Voyage created");
        Response(400, "Validation failed or vessel not found");
    }
}

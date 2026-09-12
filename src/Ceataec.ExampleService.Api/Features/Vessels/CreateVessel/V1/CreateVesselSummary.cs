using FastEndpoints;

namespace Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;

public sealed class CreateVesselSummary : Summary<CreateVesselEndpoint>
{
    public CreateVesselSummary()
    {
        Summary = "Create a vessel";
        Description = "Registers a new vessel owned by the Vessels module.";
        ExampleRequest = new CreateVesselRequest("Pacific Explorer", "IMO9123456");
        Response<CreateVesselResponse>(201, "Vessel created");
        Response(400, "Validation failed");
    }
}

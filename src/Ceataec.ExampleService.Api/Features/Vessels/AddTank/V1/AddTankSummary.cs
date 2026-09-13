using FastEndpoints;

namespace Ceataec.ExampleService.Features.Vessels.AddTank.V1;

public sealed class AddTankSummary : Summary<AddTankEndpoint>
{
    public AddTankSummary()
    {
        Summary = "Add a tank to a vessel";
        Description = "Adds a tank owned by an existing vessel in the Vessels module.";
        ExampleRequest = new AddTankRequest("Cargo 1", 1250.50m);
        Response<AddTankResponse>(201, "Tank added");
        Response(400, "Validation failed");
        Response(404, "Vessel was not found");
    }
}

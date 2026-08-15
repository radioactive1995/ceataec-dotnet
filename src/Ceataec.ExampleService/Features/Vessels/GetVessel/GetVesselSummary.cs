using FastEndpoints;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel;

public sealed class GetVesselSummary : Summary<GetVesselEndpoint>
{
    public GetVesselSummary()
    {
        Summary = "Get a vessel";
        Description = "Returns a vessel and its in-module tanks.";
        Response<GetVesselResponse>(200, "Vessel found");
        Response(404, "Vessel not found");
    }
}

using FastEndpoints;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V2;

public sealed class GetVesselSummary : Summary<GetVesselEndpoint>
{
    public GetVesselSummary()
    {
        Summary = "Get a vessel";
        Description = "Returns a vessel with aggregated tank totals instead of audit fields.";
        Response<GetVesselResponse>(200, "Vessel found");
        Response(404, "Vessel not found");
    }
}

using FastEndpoints;

namespace Ceataec.ExampleService.Features.Vessels.StreamVessels.V1;

public sealed class StreamVesselsSummary : Summary<StreamVesselsEndpoint>
{
    public StreamVesselsSummary()
    {
        Summary = "Stream all vessels";
        Description =
            "Server-sent events stream of every vessel as event 'vessel'. " +
            "Each event payload is a vessel summary without tanks. " +
            "The stream ends when the snapshot is exhausted.";
        Response(200, "Vessel event stream", "text/event-stream");
    }
}

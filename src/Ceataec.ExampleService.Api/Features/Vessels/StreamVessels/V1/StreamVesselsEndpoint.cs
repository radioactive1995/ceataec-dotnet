using FastEndpoints;

namespace Ceataec.ExampleService.Features.Vessels.StreamVessels.V1;

public sealed class StreamVesselsEndpoint : EndpointWithoutRequest
{
    public const string EventName = "vessel";

    public override void Configure()
    {
        Get("/vessels");
        Version(1);
        AllowAnonymous();
        Description(d => d.Produces<StreamVesselsResponse>(200, "text/event-stream"));
    }

    public override async Task<object?> ExecuteAsync(CancellationToken ct)
    {
        var vessels = await new StreamVesselsQuery().ExecuteAsync(ct);
        await Send.EventStreamAsync(EventName, vessels, ct);
        return null;
    }
}

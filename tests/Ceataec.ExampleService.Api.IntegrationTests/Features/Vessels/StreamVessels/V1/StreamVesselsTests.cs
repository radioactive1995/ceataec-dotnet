using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;
using Ceataec.ExampleService.Features.Vessels.StreamVessels.V1;

namespace Ceataec.ExampleService.Api.IntegrationTests.Features.Vessels.StreamVessels.V1;

[Collection(IntegrationTestCollection.Name)]
public sealed class StreamVesselsTests(ExampleWebApplicationFactory factory)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Streams_created_vessels_as_server_sent_events()
    {
        var firstId = await CreateVesselAsync("Stream Alpha");
        var secondId = await CreateVesselAsync("Stream Bravo");

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v1/vessels");
        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.StartsWith("text/event-stream", response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content.ReadAsStringAsync();
        var vessels = ParseVesselEvents(body);

        Assert.Contains(vessels, v => v.Id == firstId && v.Name == "Stream Alpha");
        Assert.Contains(vessels, v => v.Id == secondId && v.Name == "Stream Bravo");
        Assert.All(vessels, v => Assert.NotEqual(Guid.Empty, v.Id));
    }

    [Fact]
    public async Task Stream_does_not_include_non_vessel_payload_shapes()
    {
        await CreateVesselAsync("Stream Shape");

        var body = await _client.GetStringAsync("/v1/vessels");
        var vessels = ParseVesselEvents(body);

        Assert.NotEmpty(vessels);
        Assert.All(vessels, v =>
        {
            Assert.False(string.IsNullOrWhiteSpace(v.Name));
            Assert.False(string.IsNullOrWhiteSpace(v.ImoNumber));
            Assert.False(string.IsNullOrWhiteSpace(v.CreatedBy));
        });
    }

    private async Task<Guid> CreateVesselAsync(string name)
    {
        var create = await _client.PostAsJsonAsync(
            "/v1/vessels",
            new CreateVesselRequest(name, $"IMO{Random.Shared.Next(8000000, 8999999)}"));
        create.EnsureSuccessStatusCode();
        var body = await create.Content.ReadFromJsonAsync<CreateVesselResponse>();
        return body!.Id;
    }

    private static List<StreamVesselsResponse> ParseVesselEvents(string body)
    {
        var vessels = new List<StreamVesselsResponse>();
        string? pendingEvent = null;

        foreach (var rawLine in body.Split('\n'))
        {
            var line = rawLine.TrimEnd('\r');
            if (line.StartsWith("event:", StringComparison.Ordinal))
            {
                pendingEvent = line["event:".Length..].Trim();
                continue;
            }

            if (!line.StartsWith("data:", StringComparison.Ordinal))
            {
                continue;
            }

            if (pendingEvent is not null && pendingEvent != StreamVesselsEndpoint.EventName)
            {
                pendingEvent = null;
                continue;
            }

            var json = line["data:".Length..].Trim();
            var vessel = JsonSerializer.Deserialize<StreamVesselsResponse>(json, JsonOptions);
            Assert.NotNull(vessel);
            vessels.Add(vessel);
            pendingEvent = null;
        }

        return vessels;
    }
}

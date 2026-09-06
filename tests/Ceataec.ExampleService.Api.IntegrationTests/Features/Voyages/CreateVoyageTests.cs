using System.Net;
using System.Net.Http.Json;
using Ceataec.ExampleService.Features.Vessels.CreateVessel;
using Ceataec.ExampleService.Features.Voyages.CreateVoyage;

namespace Ceataec.ExampleService.Api.IntegrationTests.Features.Voyages;

[Collection(IntegrationTestCollection.Name)]
public sealed class CreateVoyageTests(ExampleWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Rejects_unknown_vessel_id()
    {
        var response = await _client.PostAsJsonAsync(
            "/voyages",
            new CreateVoyageRequest(Guid.NewGuid(), "Rotterdam", DateTimeOffset.UtcNow.AddDays(1)));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Creates_voyage_for_existing_vessel()
    {
        var vesselId = await CreateVesselAsync();

        var response = await _client.PostAsJsonAsync(
            "/voyages",
            new CreateVoyageRequest(vesselId, "Rotterdam", DateTimeOffset.UtcNow.AddDays(1)));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CreateVoyageResponse>();
        Assert.NotNull(body);
        Assert.Equal(vesselId, body.VesselId);
    }

    private async Task<Guid> CreateVesselAsync()
    {
        var create = await _client.PostAsJsonAsync(
            "/vessels",
            new CreateVesselRequest("Voyage Carrier", $"IMO{Random.Shared.Next(2000000, 2999999)}"));
        create.EnsureSuccessStatusCode();
        var body = await create.Content.ReadFromJsonAsync<CreateVesselResponse>();
        return body!.Id;
    }
}

using System.Net;
using System.Net.Http.Json;
using Ceataec.ExampleService.Features.Vessels.AddTank.V1;
using Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;
using GetVesselV1 = Ceataec.ExampleService.Features.Vessels.GetVessel.V1;

namespace Ceataec.ExampleService.Api.IntegrationTests.Features.Vessels.AddTank.V1;

[Collection(IntegrationTestCollection.Name)]
public sealed class AddTankTests(ExampleWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Adds_tank_to_existing_vessel_and_preserves_siblings()
    {
        var vesselId = await CreateVesselAsync();

        var first = await _client.PostAsJsonAsync(
            $"/v1/vessels/{vesselId}/tanks",
            new AddTankRequest("Cargo 1", 1000m));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        var firstBody = await first.Content.ReadFromJsonAsync<AddTankResponse>();
        Assert.NotNull(firstBody);
        Assert.NotEqual(Guid.Empty, firstBody.Id);
        Assert.Equal(vesselId, firstBody.VesselId);
        Assert.Equal("Cargo 1", firstBody.Name);
        Assert.Equal(1000m, firstBody.CapacityCubicMeters);
        Assert.Contains(
            $"/v1/vessels/{vesselId}/tanks/{firstBody.Id}",
            first.Headers.Location?.ToString() ?? string.Empty);

        var second = await _client.PostAsJsonAsync(
            $"/v1/vessels/{vesselId}/tanks",
            new AddTankRequest("  Cargo 2  ", 250.50m));
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);

        var get = await _client.GetAsync($"/v1/vessels/{vesselId}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        var vessel = await get.Content.ReadFromJsonAsync<GetVesselV1.GetVesselResponse>();
        Assert.NotNull(vessel);
        Assert.Equal(2, vessel.Tanks.Count);
        Assert.Contains(vessel.Tanks, t => t.Name == "Cargo 1" && t.CapacityCubicMeters == 1000m);
        Assert.Contains(vessel.Tanks, t => t.Name == "Cargo 2" && t.CapacityCubicMeters == 250.50m);
    }

    [Fact]
    public async Task Rejects_unknown_vessel_id()
    {
        var response = await _client.PostAsJsonAsync(
            $"/v1/vessels/{Guid.NewGuid()}/tanks",
            new AddTankRequest("Cargo 1", 1000m));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Rejects_non_positive_capacity()
    {
        var vesselId = await CreateVesselAsync();

        var response = await _client.PostAsJsonAsync(
            $"/v1/vessels/{vesselId}/tanks",
            new AddTankRequest("Cargo 1", 0m));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<Guid> CreateVesselAsync()
    {
        var create = await _client.PostAsJsonAsync(
            "/v1/vessels",
            new CreateVesselRequest("Tank Carrier", $"IMO{Random.Shared.Next(7000000, 7999999)}"));
        create.EnsureSuccessStatusCode();
        var body = await create.Content.ReadFromJsonAsync<CreateVesselResponse>();
        return body!.Id;
    }
}

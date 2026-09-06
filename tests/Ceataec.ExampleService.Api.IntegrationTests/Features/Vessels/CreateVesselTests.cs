using System.Net;
using System.Net.Http.Json;
using Ceataec.ExampleService.Features.Vessels.CreateVessel;
using Ceataec.ExampleService.Features.Vessels.GetVessel;

namespace Ceataec.ExampleService.Api.IntegrationTests.Features.Vessels;

[Collection(IntegrationTestCollection.Name)]
public sealed class CreateVesselTests(ExampleWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Create_then_get_vessel()
    {
        var create = await _client.PostAsJsonAsync(
            "/vessels",
            new CreateVesselRequest("Atlantic Star", "IMO1000001"));

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var created = await create.Content.ReadFromJsonAsync<CreateVesselResponse>();
        Assert.NotNull(created);

        var get = await _client.GetAsync($"/vessels/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var vessel = await get.Content.ReadFromJsonAsync<GetVesselResponse>();
        Assert.NotNull(vessel);
        Assert.Equal("Atlantic Star", vessel.Name);
        Assert.Equal("test-user", vessel.CreatedBy);
    }
}

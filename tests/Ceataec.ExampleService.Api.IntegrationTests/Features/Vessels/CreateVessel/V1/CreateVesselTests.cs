using System.Net;
using System.Net.Http.Json;
using Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;

namespace Ceataec.ExampleService.Api.IntegrationTests.Features.Vessels.CreateVessel.V1;

[Collection(IntegrationTestCollection.Name)]
public sealed class CreateVesselTests(ExampleWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Creates_vessel()
    {
        var create = await _client.PostAsJsonAsync(
            "/v1/vessels",
            new CreateVesselRequest("Atlantic Star", "IMO1000001"));

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var created = await create.Content.ReadFromJsonAsync<CreateVesselResponse>();
        Assert.NotNull(created);
        Assert.Equal("Atlantic Star", created.Name);
        Assert.Equal("IMO1000001", created.ImoNumber);
        Assert.Equal("test-user", created.CreatedBy);
        Assert.NotEqual(Guid.Empty, created.Id);
    }
}

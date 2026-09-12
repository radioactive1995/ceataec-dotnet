using System.Net;
using System.Net.Http.Json;
using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;
using GetVesselV1 = Ceataec.ExampleService.Features.Vessels.GetVessel.V1;
using GetVesselV2 = Ceataec.ExampleService.Features.Vessels.GetVessel.V2;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ceataec.ExampleService.Api.IntegrationTests.Features.Vessels.GetVessel;

[Collection(IntegrationTestCollection.Name)]
public sealed class GetVesselVersionTests(ExampleWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Both_versions_of_the_vessel_route_serve_their_own_contract()
    {
        var create = await _client.PostAsJsonAsync(
            "/v1/vessels",
            new CreateVesselRequest("Nordic Dawn", $"IMO{Random.Shared.Next(4000000, 4999999)}"));
        create.EnsureSuccessStatusCode();

        var created = await create.Content.ReadFromJsonAsync<CreateVesselResponse>();
        Assert.NotNull(created);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ICommandDbContext>();
            var vessel = await db.Set<Vessel>().SingleAsync(v => v.Id == created.Id);
            vessel.AddTank("Cargo 1", 1000m);
            vessel.AddTank("Cargo 2", 250.50m);
            await db.SaveChangesAsync();
        }

        var v1 = await _client.GetAsync($"/v1/vessels/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, v1.StatusCode);
        var v1Body = await v1.Content.ReadFromJsonAsync<GetVesselV1.GetVesselResponse>();
        Assert.NotNull(v1Body);
        Assert.Equal("test-user", v1Body.CreatedBy);
        Assert.Equal(2, v1Body.Tanks.Count);

        var v2 = await _client.GetAsync($"/v2/vessels/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, v2.StatusCode);
        var v2Body = await v2.Content.ReadFromJsonAsync<GetVesselV2.GetVesselResponse>();
        Assert.NotNull(v2Body);
        Assert.Equal("Nordic Dawn", v2Body.Name);
        Assert.Equal(2, v2Body.TankCount);
        Assert.Equal(1250.50m, v2Body.TotalCapacityCubicMeters);
    }
}

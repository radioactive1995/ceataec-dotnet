using System.Net;
using System.Net.Http.Json;
using Ceataec.ExampleService.Features.Vessels.CreateVessel;
using Ceataec.ExampleService.Features.Vessels.GetVessel;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var storedVessel = await db.Vessels.SingleAsync(v => v.Id == created.Id);
            storedVessel.AddTank("Cargo 1", 1250.50m);
            await db.SaveChangesAsync();
        }

        var get = await _client.GetAsync($"/vessels/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var vessel = await get.Content.ReadFromJsonAsync<GetVesselResponse>();
        Assert.NotNull(vessel);
        Assert.Equal("Atlantic Star", vessel.Name);
        Assert.Equal("test-user", vessel.CreatedBy);
        var tank = Assert.Single(vessel.Tanks);
        Assert.Equal("Cargo 1", tank.Name);
        Assert.Equal(1250.50m, tank.CapacityCubicMeters);
    }
}

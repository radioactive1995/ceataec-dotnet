using System.Net;
using System.Net.Http.Json;
using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;
using Ceataec.ExampleService.Features.Vessels.GetVessel.V1;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ceataec.ExampleService.Api.IntegrationTests.Features.Vessels.GetVessel.V1;

[Collection(IntegrationTestCollection.Name)]
public sealed class GetVesselTests(ExampleWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Returns_vessel_with_tanks_and_audit_fields()
    {
        var create = await _client.PostAsJsonAsync(
            "/v1/vessels",
            new CreateVesselRequest("Atlantic Star", $"IMO{Random.Shared.Next(1000000, 1999999)}"));
        create.EnsureSuccessStatusCode();

        var created = await create.Content.ReadFromJsonAsync<CreateVesselResponse>();
        Assert.NotNull(created);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ICommandDbContext>();
            var storedVessel = await db.Set<Vessel>().SingleAsync(v => v.Id == created.Id);
            storedVessel.AddTank("Cargo 1", 1250.50m);
            await db.SaveChangesAsync();
        }

        var get = await _client.GetAsync($"/v1/vessels/{created.Id}");
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

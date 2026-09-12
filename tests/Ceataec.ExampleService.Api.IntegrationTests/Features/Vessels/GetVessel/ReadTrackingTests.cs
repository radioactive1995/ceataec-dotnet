using System.Net.Http.Json;
using Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Infrastructure.Persistence.Vessels.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Ceataec.ExampleService.Api.IntegrationTests.Features.Vessels.GetVessel;

[Collection(IntegrationTestCollection.Name)]
public sealed class ReadTrackingTests(ExampleWebApplicationFactory factory)
{
    [Fact]
    public async Task Named_read_returns_an_existing_aggregate_without_tracking_it()
    {
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/v1/vessels",
            new CreateVesselRequest("Read tracking test", $"IMO{Random.Shared.Next(6000000, 6999999)}"));
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<CreateVesselResponse>();
        Assert.NotNull(created);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(db.ChangeTracker.Entries());

        var vessel = await GetVesselWithTanks.QueryAsync(db, created.Id, CancellationToken.None);

        Assert.NotNull(vessel);
        Assert.Equal(created.Id, vessel.Id);
        Assert.Empty(db.ChangeTracker.Entries());
    }
}

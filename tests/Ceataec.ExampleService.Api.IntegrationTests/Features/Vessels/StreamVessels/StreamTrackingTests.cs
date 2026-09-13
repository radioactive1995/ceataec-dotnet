using System.Net.Http.Json;
using Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Infrastructure.Persistence.Vessels.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Ceataec.ExampleService.Api.IntegrationTests.Features.Vessels.StreamVessels;

[Collection(IntegrationTestCollection.Name)]
public sealed class StreamTrackingTests(ExampleWebApplicationFactory factory)
{
    [Fact]
    public async Task Named_stream_reads_existing_vessels_without_tracking_them()
    {
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/v1/vessels",
            new CreateVesselRequest("Stream tracking test", $"IMO{Random.Shared.Next(8100000, 8199999)}"));
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<CreateVesselResponse>();
        Assert.NotNull(created);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(db.ChangeTracker.Entries());

        var found = false;
        await foreach (var vessel in await StreamAllVessels.QueryAsync(db, null, CancellationToken.None))
        {
            Assert.Empty(db.ChangeTracker.Entries());
            if (vessel.Id == created.Id)
            {
                found = true;
            }
        }

        Assert.True(found);
        Assert.Empty(db.ChangeTracker.Entries());
    }
}

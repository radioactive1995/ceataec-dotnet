using System.Net;
using System.Net.Http.Json;
using Ceataec.ExampleService.Features.Certificates.CreateCertificate.V1;
using Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;

namespace Ceataec.ExampleService.Api.IntegrationTests.Features.Certificates.CreateCertificate.V1;

[Collection(IntegrationTestCollection.Name)]
public sealed class CreateCertificateTests(ExampleWebApplicationFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Rejects_unknown_vessel_id()
    {
        var response = await _client.PostAsJsonAsync(
            "/v1/certificates",
            new CreateCertificateRequest(Guid.NewGuid(), "Safety Management", DateTimeOffset.UtcNow));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Creates_certificate_for_existing_vessel()
    {
        var vesselId = await CreateVesselAsync();

        var response = await _client.PostAsJsonAsync(
            "/v1/certificates",
            new CreateCertificateRequest(vesselId, "Safety Management", DateTimeOffset.UtcNow));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CreateCertificateResponse>();
        Assert.NotNull(body);
        Assert.Equal(vesselId, body.VesselId);
    }

    private async Task<Guid> CreateVesselAsync()
    {
        var create = await _client.PostAsJsonAsync(
            "/v1/vessels",
            new CreateVesselRequest("Cert Carrier", $"IMO{Random.Shared.Next(3000000, 3999999)}"));
        create.EnsureSuccessStatusCode();
        var body = await create.Content.ReadFromJsonAsync<CreateVesselResponse>();
        return body!.Id;
    }
}

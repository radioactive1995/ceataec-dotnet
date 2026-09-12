using FastEndpoints;

namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate.V1;

public sealed class CreateCertificateSummary : Summary<CreateCertificateEndpoint>
{
    public CreateCertificateSummary()
    {
        Summary = "Create a certificate";
        Description = "Registers a compliance certificate for an existing vessel. Stores VesselId only — no cross-module SQL FK.";
        ExampleRequest = new CreateCertificateRequest(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Safety Management",
            DateTimeOffset.UtcNow);
        Response<CreateCertificateResponse>(201, "Certificate created");
        Response(400, "Validation failed or vessel not found");
    }
}

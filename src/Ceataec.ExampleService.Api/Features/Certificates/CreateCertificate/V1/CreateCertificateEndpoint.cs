using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate.V1;

public sealed class CreateCertificateEndpoint
    : Endpoint<CreateCertificateRequest, Results<Created<CreateCertificateResponse>, NotFound>>
{
    public override void Configure()
    {
        Post("/certificates");
        Version(1);
        AllowAnonymous();
    }

    public override async Task<Results<Created<CreateCertificateResponse>, NotFound>> ExecuteAsync(
        CreateCertificateRequest req,
        CancellationToken ct)
    {
        var response = await new CreateCertificateCommand(
                req.VesselId,
                req.Type,
                req.IssuedOn)
            .ExecuteAsync(ct);

        if (response is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Created($"/v1/certificates/{response.Id}", response);
    }
}

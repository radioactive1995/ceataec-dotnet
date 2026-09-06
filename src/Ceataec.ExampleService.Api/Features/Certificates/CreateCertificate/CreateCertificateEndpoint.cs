using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate;

public sealed class CreateCertificateEndpoint
    : Endpoint<CreateCertificateRequest, Results<Created<CreateCertificateResponse>, NotFound>>
{
    public override void Configure()
    {
        Post("/certificates");
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

        return TypedResults.Created($"/certificates/{response.Id}", response);
    }
}

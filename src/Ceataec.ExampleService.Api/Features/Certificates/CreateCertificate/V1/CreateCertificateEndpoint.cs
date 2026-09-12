using Ceataec.ExampleService.Http;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate.V1;

public sealed class CreateCertificateEndpoint
    : Endpoint<CreateCertificateRequest, Results<Created<CreateCertificateResponse>, ProblemHttpResult>>
{
    public override void Configure()
    {
        Post("/certificates");
        Version(1);
        AllowAnonymous();
    }

    public override async Task<Results<Created<CreateCertificateResponse>, ProblemHttpResult>> ExecuteAsync(
        CreateCertificateRequest req,
        CancellationToken ct)
    {
        var result = await new CreateCertificateCommand(
                req.VesselId,
                req.Type,
                req.IssuedOn)
            .ExecuteAsync(ct);

        return result.Match<Results<Created<CreateCertificateResponse>, ProblemHttpResult>>(
            response => TypedResults.Created($"/v1/certificates/{response.Id}", response),
            errors => errors.ToProblem());
    }
}

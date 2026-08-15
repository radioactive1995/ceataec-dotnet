using Ceataec.ExampleService.Domain.Certificates;
using Ceataec.ExampleService.Infrastructure.Persistence.Certificates;
using Ceataec.ExampleService.Infrastructure.Persistence.Vessels;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate;

public sealed class CreateCertificateEndpoint(
    ICertificateRepository certificates,
    IVesselRepository vessels,
    ILogger<CreateCertificateEndpoint> logger)
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
        if (!await vessels.ExistsAsync(req.VesselId, ct))
        {
            logger.LogWarning("Rejected certificate for unknown vessel {VesselId}", req.VesselId);
            return TypedResults.NotFound();
        }

        var certificate = new Certificate
        {
            Id = Guid.NewGuid(),
            VesselId = req.VesselId,
            Type = req.Type.Trim(),
            IssuedOn = req.IssuedOn
        };

        await certificates.AddAsync(certificate, ct);

        var response = new CreateCertificateResponse(
            certificate.Id,
            certificate.VesselId,
            certificate.Type,
            certificate.IssuedOn);

        return TypedResults.Created($"/certificates/{certificate.Id}", response);
    }
}

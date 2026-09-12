using Ceataec.ExampleService.Domain.Certificates;
using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Infrastructure.Persistence;
using ErrorOr;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate.V1;

public sealed class CreateCertificateHandler(
    ICommandDbContext dbContext,
    ILogger<CreateCertificateHandler> logger)
    : ICommandHandler<CreateCertificateCommand, ErrorOr<CreateCertificateResponse>>
{
    public async Task<ErrorOr<CreateCertificateResponse>> ExecuteAsync(
        CreateCertificateCommand command,
        CancellationToken ct)
    {
        var vesselExists = await dbContext.Set<Vessel>()
            .AnyAsync(v => v.Id == command.VesselId, ct);

        if (!vesselExists)
        {
            logger.LogWarning("Rejected certificate for unknown vessel {VesselId}", command.VesselId);
            return Error.NotFound("vessel_not_found", "Vessel was not found.");
        }

        var certificate = Certificate.Create(
            command.VesselId,
            command.Type,
            command.IssuedOn);

        if (certificate.IsError)
        {
            return certificate.Errors;
        }

        dbContext.Set<Certificate>().Add(certificate.Value);
        await dbContext.SaveChangesAsync(ct);

        return new CreateCertificateResponse(
            certificate.Value.Id,
            certificate.Value.VesselId,
            certificate.Value.Type,
            certificate.Value.IssuedOn);
    }
}

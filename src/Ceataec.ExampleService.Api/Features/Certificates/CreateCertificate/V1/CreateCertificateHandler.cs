using Ceataec.ExampleService.Domain.Certificates;
using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Infrastructure.Persistence;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate.V1;

public sealed class CreateCertificateHandler(
    ICommandDbContext dbContext,
    ILogger<CreateCertificateHandler> logger)
    : ICommandHandler<CreateCertificateCommand, CreateCertificateResponse?>
{
    public async Task<CreateCertificateResponse?> ExecuteAsync(
        CreateCertificateCommand command,
        CancellationToken ct)
    {
        var vesselExists = await dbContext.Set<Vessel>()
            .AnyAsync(v => v.Id == command.VesselId, ct);

        if (!vesselExists)
        {
            logger.LogWarning("Rejected certificate for unknown vessel {VesselId}", command.VesselId);
            return null;
        }

        var certificate = Certificate.Create(
            command.VesselId,
            command.Type,
            command.IssuedOn);

        dbContext.Set<Certificate>().Add(certificate);
        await dbContext.SaveChangesAsync(ct);

        return new CreateCertificateResponse(
            certificate.Id,
            certificate.VesselId,
            certificate.Type,
            certificate.IssuedOn);
    }
}

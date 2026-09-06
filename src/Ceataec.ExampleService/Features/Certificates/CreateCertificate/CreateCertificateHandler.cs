using Ceataec.ExampleService.Domain.Certificates;
using Ceataec.ExampleService.Infrastructure.Persistence;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate;

public sealed class CreateCertificateHandler(
    AppDbContext dbContext,
    ILogger<CreateCertificateHandler> logger)
    : ICommandHandler<CreateCertificateCommand, CreateCertificateResponse?>
{
    public async Task<CreateCertificateResponse?> ExecuteAsync(
        CreateCertificateCommand command,
        CancellationToken ct)
    {
        var vesselExists = await dbContext.Vessels
            .AnyAsync(v => v.Id == command.VesselId, ct);

        if (!vesselExists)
        {
            logger.LogWarning("Rejected certificate for unknown vessel {VesselId}", command.VesselId);
            return null;
        }

        var certificate = new Certificate
        {
            Id = Guid.NewGuid(),
            VesselId = command.VesselId,
            Type = command.Type.Trim(),
            IssuedOn = command.IssuedOn
        };

        dbContext.Certificates.Add(certificate);
        await dbContext.SaveChangesAsync(ct);

        return new CreateCertificateResponse(
            certificate.Id,
            certificate.VesselId,
            certificate.Type,
            certificate.IssuedOn);
    }
}

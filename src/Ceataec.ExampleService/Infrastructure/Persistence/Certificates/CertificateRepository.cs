using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Domain.Certificates;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Certificates;

public sealed class CertificateRepository(AppDbContext dbContext) : ICertificateRepository
{
    public async Task AddAsync(Certificate certificate, CancellationToken cancellationToken)
    {
        dbContext.Certificates.Add(certificate);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

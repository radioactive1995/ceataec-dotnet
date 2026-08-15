using Ceataec.ExampleService.Domain.Certificates;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Certificates;

public interface ICertificateRepository
{
    Task AddAsync(Certificate certificate, CancellationToken cancellationToken);
}

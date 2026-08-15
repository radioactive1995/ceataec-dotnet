using Ceataec.ExampleService.Domain.Vessels;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Vessels;

public interface IVesselRepository
{
    Task AddAsync(Vessel vessel, CancellationToken cancellationToken);
    Task<Vessel?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
}

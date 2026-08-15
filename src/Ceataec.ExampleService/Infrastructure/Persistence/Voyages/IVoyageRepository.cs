using Ceataec.ExampleService.Domain.Voyages;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Voyages;

public interface IVoyageRepository
{
    Task AddAsync(Voyage voyage, CancellationToken cancellationToken);
}

using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Domain.Voyages;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Voyages;

public sealed class VoyageRepository(AppDbContext dbContext) : IVoyageRepository
{
    public async Task AddAsync(Voyage voyage, CancellationToken cancellationToken)
    {
        dbContext.Voyages.Add(voyage);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

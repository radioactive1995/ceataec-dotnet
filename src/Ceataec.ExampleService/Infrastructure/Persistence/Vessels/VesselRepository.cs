using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Domain.Vessels;
using Microsoft.EntityFrameworkCore;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Vessels;

public sealed class VesselRepository(AppDbContext dbContext) : IVesselRepository
{
    public async Task AddAsync(Vessel vessel, CancellationToken cancellationToken)
    {
        dbContext.Vessels.Add(vessel);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Vessel?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.Vessels
            .Include(v => v.Tanks)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        => dbContext.Vessels.AnyAsync(v => v.Id == id, cancellationToken);
}

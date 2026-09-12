using Ceataec.ExampleService.Domain.Vessels;
using Microsoft.EntityFrameworkCore;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Vessels.Queries;

public sealed class GetVesselWithTanks : IDbQuery<Guid, Vessel?>
{
    public static Task<Vessel?> QueryAsync(
        AppDbContext dbContext,
        Guid vesselId,
        CancellationToken cancellationToken)
        => dbContext.Set<Vessel>()
            .Include(v => v.Tanks)
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == vesselId, cancellationToken);
}

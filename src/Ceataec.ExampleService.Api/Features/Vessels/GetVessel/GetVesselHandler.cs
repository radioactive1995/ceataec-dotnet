using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel;

public sealed class GetVesselHandler(AppDbContext dbContext)
    : IQueryHandler<GetVesselQuery, GetVesselResponse?>
{
    public async Task<GetVesselResponse?> ExecuteAsync(
        GetVesselQuery query,
        CancellationToken ct)
    {
        var vessel = await dbContext.Vessels
            .Include(v => v.Tanks)
            .FirstOrDefaultAsync(v => v.Id == query.Id, ct);

        if (vessel is null)
        {
            return null;
        }

        return new GetVesselResponse(
            vessel.Id,
            vessel.Name,
            vessel.ImoNumber,
            vessel.CreatedBy,
            vessel.Tanks
                .Select(t => new TankDto(t.Id, t.Name, t.CapacityCubicMeters))
                .ToList());
    }
}

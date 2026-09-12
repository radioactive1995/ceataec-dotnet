using Ceataec.ExampleService.Cqrs;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Infrastructure.Persistence.Vessels.Queries;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V1;

public sealed class GetVesselHandler(AppDbContext dbContext)
    : IQueryHandler<GetVesselQuery, GetVesselResponse?>
{
    public async Task<GetVesselResponse?> ExecuteAsync(
        GetVesselQuery query,
        CancellationToken ct)
    {
        var vessel = await GetVesselWithTanks.QueryAsync(dbContext, query.Id, ct);

        if (vessel is null)
        {
            return null;
        }

        return new GetVesselResponse(
            vessel.Id,
            vessel.Name,
            vessel.ImoNumber.Value,
            vessel.CreatedBy,
            vessel.Tanks
                .Select(t => new TankDto(t.Id, t.Name, t.CapacityCubicMeters))
                .ToList());
    }
}

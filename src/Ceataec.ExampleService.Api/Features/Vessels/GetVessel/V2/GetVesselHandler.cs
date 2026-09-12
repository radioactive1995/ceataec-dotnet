using Ceataec.ExampleService.Cqrs;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Infrastructure.Persistence.Vessels.Queries;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V2;

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
            vessel.Tanks.Count,
            vessel.Tanks.Sum(t => t.CapacityCubicMeters));
    }
}

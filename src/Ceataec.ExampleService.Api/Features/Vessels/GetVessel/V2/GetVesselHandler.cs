using Ceataec.ExampleService.Cqrs;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Infrastructure.Persistence.Vessels.Queries;
using ErrorOr;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V2;

public sealed class GetVesselHandler(AppDbContext dbContext)
    : IQueryHandler<GetVesselQuery, ErrorOr<GetVesselResponse>>
{
    public async Task<ErrorOr<GetVesselResponse>> ExecuteAsync(
        GetVesselQuery query,
        CancellationToken ct)
    {
        var vessel = await GetVesselWithTanks.QueryAsync(dbContext, query.Id, ct);

        if (vessel is null)
        {
            return Error.NotFound("vessel_not_found", "Vessel was not found.");
        }

        return new GetVesselResponse(
            vessel.Id,
            vessel.Name,
            vessel.ImoNumber.Value,
            vessel.Tanks.Count,
            vessel.Tanks.Sum(t => t.CapacityCubicMeters));
    }
}

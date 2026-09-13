using Ceataec.ExampleService.Cqrs;
using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Infrastructure.Persistence.Vessels.Queries;
using System.Runtime.CompilerServices;

namespace Ceataec.ExampleService.Features.Vessels.StreamVessels.V1;

public sealed class StreamVesselsHandler(AppDbContext dbContext)
    : IQueryHandler<StreamVesselsQuery, IAsyncEnumerable<StreamVesselsResponse>>
{
    public Task<IAsyncEnumerable<StreamVesselsResponse>> ExecuteAsync(
        StreamVesselsQuery query,
        CancellationToken ct)
        => Task.FromResult(Stream(ct));

    private async IAsyncEnumerable<StreamVesselsResponse> Stream(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var vessels = await StreamAllVessels.QueryAsync(dbContext, null, cancellationToken);

        await foreach (var vessel in vessels.WithCancellation(cancellationToken))
        {
            yield return ToResponse(vessel);
        }
    }

    private static StreamVesselsResponse ToResponse(Vessel vessel)
        => new(vessel.Id, vessel.Name, vessel.ImoNumber.Value, vessel.CreatedBy);
}

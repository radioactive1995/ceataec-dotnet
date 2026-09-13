using Ceataec.ExampleService.Domain.Vessels;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Ceataec.ExampleService.Infrastructure.Persistence.Vessels.Queries;

public sealed class StreamAllVessels : IDbQuery<object?, IAsyncEnumerable<Vessel>>
{
    public static Task<IAsyncEnumerable<Vessel>> QueryAsync(
        AppDbContext dbContext,
        object? input,
        CancellationToken cancellationToken)
        => Task.FromResult(Stream(dbContext, cancellationToken));

    private static async IAsyncEnumerable<Vessel> Stream(
        AppDbContext dbContext,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var vessel in dbContext.Set<Vessel>()
            .AsNoTracking()
            .OrderBy(v => v.Id)
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            yield return vessel;
        }
    }
}

using Ceataec.ExampleService.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ceataec.ExampleService.Infrastructure.Persistence;

/// <summary>
/// Provides command handlers with persistence access restricted to aggregate roots.
/// </summary>
public interface ICommandDbContext
{
    /// <summary>
    /// Gets the set used to query and persist an aggregate root type.
    /// </summary>
    /// <typeparam name="TAggregateRoot">
    /// The aggregate root type.
    /// </typeparam>
    /// <returns>
    /// The set for the requested aggregate root type.
    /// </returns>
    DbSet<TAggregateRoot> Set<TAggregateRoot>()
        where TAggregateRoot : AggregateRoot;

    /// <summary>
    /// Saves all tracked changes to the database.
    /// </summary>
    /// <param name="cancellationToken">
    /// The token used to cancel the operation.
    /// </param>
    /// <returns>
    /// The number of state entries written to the database.
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

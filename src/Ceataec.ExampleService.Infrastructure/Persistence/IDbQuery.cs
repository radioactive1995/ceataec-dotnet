namespace Ceataec.ExampleService.Infrastructure.Persistence;

/// <summary>
/// Defines a reusable database query that executes without change tracking.
/// </summary>
/// <typeparam name="TInput">
/// The query input type.
/// </typeparam>
/// <typeparam name="TResult">
/// The unrestricted result type, such as an entity, value, or projection.
/// </typeparam>
public interface IDbQuery<TInput, TResult>
{
    /// <summary>
    /// Executes the query against the application database.
    /// </summary>
    /// <param name="dbContext">
    /// The unrestricted database context used for read access.
    /// </param>
    /// <param name="input">
    /// The input used to select or shape the result.
    /// </param>
    /// <param name="cancellationToken">
    /// The token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task containing the query result.
    /// </returns>
    static abstract Task<TResult> QueryAsync(
        AppDbContext dbContext,
        TInput input,
        CancellationToken cancellationToken);
}

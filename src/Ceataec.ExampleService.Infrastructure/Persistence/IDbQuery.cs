namespace Ceataec.ExampleService.Infrastructure.Persistence;

public interface IDbQuery<TInput, TResult>
{
    static abstract Task<TResult> QueryAsync(
        AppDbContext dbContext,
        TInput input,
        CancellationToken cancellationToken);
}

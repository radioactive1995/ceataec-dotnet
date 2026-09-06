using FastEndpoints;

namespace Ceataec.ExampleService.Cqrs;

public interface IQueryHandler<TQuery, TResult> : ICommandHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>;

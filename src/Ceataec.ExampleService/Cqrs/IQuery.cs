using FastEndpoints;

namespace Ceataec.ExampleService.Cqrs;

public interface IQuery<TResult> : ICommand<TResult>;

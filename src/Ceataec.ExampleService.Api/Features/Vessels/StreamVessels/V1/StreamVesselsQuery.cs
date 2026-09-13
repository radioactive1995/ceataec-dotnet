using Ceataec.ExampleService.Cqrs;

namespace Ceataec.ExampleService.Features.Vessels.StreamVessels.V1;

public sealed record StreamVesselsQuery : IQuery<IAsyncEnumerable<StreamVesselsResponse>>;

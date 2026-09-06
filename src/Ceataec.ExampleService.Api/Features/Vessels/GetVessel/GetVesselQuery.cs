using Ceataec.ExampleService.Cqrs;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel;

public sealed record GetVesselQuery(Guid Id) : IQuery<GetVesselResponse?>;

using Ceataec.ExampleService.Cqrs;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V1;

public sealed record GetVesselQuery(Guid Id) : IQuery<GetVesselResponse?>;

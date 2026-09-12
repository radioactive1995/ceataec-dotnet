using Ceataec.ExampleService.Cqrs;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V2;

public sealed record GetVesselQuery(Guid Id) : IQuery<GetVesselResponse?>;

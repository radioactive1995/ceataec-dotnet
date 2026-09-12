using Ceataec.ExampleService.Cqrs;
using ErrorOr;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V2;

public sealed record GetVesselQuery(Guid Id) : IQuery<ErrorOr<GetVesselResponse>>;

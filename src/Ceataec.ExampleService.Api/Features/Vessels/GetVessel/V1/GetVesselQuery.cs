using Ceataec.ExampleService.Cqrs;
using ErrorOr;

namespace Ceataec.ExampleService.Features.Vessels.GetVessel.V1;

public sealed record GetVesselQuery(Guid Id) : IQuery<ErrorOr<GetVesselResponse>>;

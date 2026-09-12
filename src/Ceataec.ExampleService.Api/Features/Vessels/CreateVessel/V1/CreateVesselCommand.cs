using FastEndpoints;

namespace Ceataec.ExampleService.Features.Vessels.CreateVessel.V1;

public sealed record CreateVesselCommand(string Name, string ImoNumber)
    : ICommand<CreateVesselResponse>;

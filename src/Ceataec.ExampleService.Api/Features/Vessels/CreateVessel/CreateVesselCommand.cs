using FastEndpoints;

namespace Ceataec.ExampleService.Features.Vessels.CreateVessel;

public sealed record CreateVesselCommand(string Name, string ImoNumber)
    : ICommand<CreateVesselResponse>;

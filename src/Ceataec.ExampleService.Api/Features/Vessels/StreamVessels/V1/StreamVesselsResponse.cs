namespace Ceataec.ExampleService.Features.Vessels.StreamVessels.V1;

public sealed record StreamVesselsResponse(
    Guid Id,
    string Name,
    string ImoNumber,
    string CreatedBy);

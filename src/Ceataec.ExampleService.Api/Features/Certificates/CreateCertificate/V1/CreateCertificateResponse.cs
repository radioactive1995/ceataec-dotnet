namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate.V1;

public sealed record CreateCertificateResponse(
    Guid Id,
    Guid VesselId,
    string Type,
    DateTimeOffset IssuedOn);

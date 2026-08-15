namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate;

public sealed record CreateCertificateResponse(
    Guid Id,
    Guid VesselId,
    string Type,
    DateTimeOffset IssuedOn);

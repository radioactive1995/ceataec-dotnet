namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate;

public sealed record CreateCertificateRequest(
    Guid VesselId,
    string Type,
    DateTimeOffset IssuedOn);

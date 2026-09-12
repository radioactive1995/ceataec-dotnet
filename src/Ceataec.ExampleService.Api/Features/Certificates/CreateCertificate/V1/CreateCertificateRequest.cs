namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate.V1;

public sealed record CreateCertificateRequest(
    Guid VesselId,
    string Type,
    DateTimeOffset IssuedOn);

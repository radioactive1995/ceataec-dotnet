using ErrorOr;
using FastEndpoints;

namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate.V1;

public sealed record CreateCertificateCommand(
    Guid VesselId,
    string Type,
    DateTimeOffset IssuedOn) : ICommand<ErrorOr<CreateCertificateResponse>>;

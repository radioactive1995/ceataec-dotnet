using FastEndpoints;
using FluentValidation;

namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate;

public sealed class CreateCertificateValidator : Validator<CreateCertificateRequest>
{
    public CreateCertificateValidator()
    {
        RuleFor(x => x.VesselId).NotEmpty();
        RuleFor(x => x.Type).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IssuedOn).NotEmpty();
    }
}

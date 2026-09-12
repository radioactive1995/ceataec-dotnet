using Ceataec.ExampleService.Domain.Certificates;
using FastEndpoints;
using FluentValidation;

namespace Ceataec.ExampleService.Features.Certificates.CreateCertificate;

public sealed class CreateCertificateValidator : Validator<CreateCertificateRequest>
{
    public CreateCertificateValidator()
    {
        RuleFor(x => x.VesselId).NotEmpty();
        RuleFor(x => x.Type).NotEmpty().MaximumLength(Certificate.TypeMaxLength);
        RuleFor(x => x.IssuedOn).NotEmpty();
    }
}

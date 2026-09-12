using ErrorOr;

namespace Ceataec.ExampleService.Domain.Certificates;

public sealed class Certificate : AggregateRoot
{
    public const int TypeMaxLength = 100;

    private Certificate()
    {
    }

    public Guid VesselId { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public DateTimeOffset IssuedOn { get; private set; }

    public static ErrorOr<Certificate> Create(Guid vesselId, string type, DateTimeOffset issuedOn)
    {
        if (vesselId == Guid.Empty)
        {
            return Error.Validation("vessel_id_required", "Vessel id is required.");
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            return Error.Validation("certificate_type_required", "Certificate type is required.");
        }

        var trimmedType = type.Trim();
        if (trimmedType.Length > TypeMaxLength)
        {
            return Error.Validation(
                "certificate_type_too_long",
                $"Certificate type must be at most {TypeMaxLength} characters.");
        }

        if (issuedOn == default)
        {
            return Error.Validation("certificate_issue_date_required", "Issue date is required.");
        }

        return new Certificate
        {
            VesselId = vesselId,
            Type = trimmedType,
            IssuedOn = issuedOn
        };
    }
}

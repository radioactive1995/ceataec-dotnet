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

    public static Certificate Create(Guid vesselId, string type, DateTimeOffset issuedOn)
    {
        if (vesselId == Guid.Empty)
        {
            throw new DomainException("Vessel id is required.", "vessel_id_required");
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            throw new DomainException("Certificate type is required.", "certificate_type_required");
        }

        var trimmedType = type.Trim();
        if (trimmedType.Length > TypeMaxLength)
        {
            throw new DomainException(
                $"Certificate type must be at most {TypeMaxLength} characters.",
                "certificate_type_too_long");
        }

        if (issuedOn == default)
        {
            throw new DomainException("Issue date is required.", "certificate_issue_date_required");
        }

        return new Certificate
        {
            VesselId = vesselId,
            Type = trimmedType,
            IssuedOn = issuedOn
        };
    }
}

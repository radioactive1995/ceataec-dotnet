namespace Ceataec.ExampleService.Domain.Vessels.ValueObjects;

public readonly record struct ImoNumber
{
    public const int MaxLength = 20;

    public string Value { get; }

    private ImoNumber(string value) => Value = value;

    public static ImoNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("IMO number is required.", "imo_number_required");
        }

        var trimmed = value.Trim();
        if (trimmed.Length > MaxLength)
        {
            throw new DomainException(
                $"IMO number must be at most {MaxLength} characters.",
                "imo_number_too_long");
        }

        return new ImoNumber(trimmed);
    }

    /// <summary>Rehydrate from persistence without re-validating (trusted store).</summary>
    public static ImoNumber FromPersistence(string value) => new(value);

    public override string ToString() => Value;
}

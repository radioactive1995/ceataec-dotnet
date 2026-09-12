using ErrorOr;

namespace Ceataec.ExampleService.Domain.Vessels.ValueObjects;

public readonly record struct ImoNumber
{
    public const int MaxLength = 20;

    public string Value { get; }

    private ImoNumber(string value) => Value = value;

    public static ErrorOr<ImoNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation("imo_number_required", "IMO number is required.");
        }

        var trimmed = value.Trim();
        if (trimmed.Length > MaxLength)
        {
            return Error.Validation(
                "imo_number_too_long",
                $"IMO number must be at most {MaxLength} characters.");
        }

        return new ImoNumber(trimmed);
    }

    /// <summary>Rehydrate from persistence without re-validating (trusted store).</summary>
    public static ImoNumber FromPersistence(string value) => new(value);

    public override string ToString() => Value;
}

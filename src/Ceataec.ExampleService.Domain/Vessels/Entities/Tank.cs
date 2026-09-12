using ErrorOr;

namespace Ceataec.ExampleService.Domain.Vessels.Entities;

public sealed class Tank : Entity
{
    public const int NameMaxLength = 100;

    private Tank()
    {
    }

    public Guid VesselId { get; private set; }
    public Vessel Vessel { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public decimal CapacityCubicMeters { get; private set; }

    internal static ErrorOr<Tank> Create(Vessel vessel, string name, decimal capacityCubicMeters)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("tank_name_required", "Tank name is required.");
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > NameMaxLength)
        {
            return Error.Validation(
                "tank_name_too_long",
                $"Tank name must be at most {NameMaxLength} characters.");
        }

        if (capacityCubicMeters <= 0)
        {
            return Error.Validation(
                "tank_capacity_invalid",
                "Tank capacity must be greater than zero.");
        }

        return new Tank
        {
            VesselId = vessel.Id,
            Vessel = vessel,
            Name = trimmedName,
            CapacityCubicMeters = capacityCubicMeters
        };
    }
}

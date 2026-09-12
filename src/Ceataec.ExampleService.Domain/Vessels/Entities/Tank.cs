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

    internal static Tank Create(Vessel vessel, string name, decimal capacityCubicMeters)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Tank name is required.", "tank_name_required");
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > NameMaxLength)
        {
            throw new DomainException(
                $"Tank name must be at most {NameMaxLength} characters.",
                "tank_name_too_long");
        }

        if (capacityCubicMeters <= 0)
        {
            throw new DomainException(
                "Tank capacity must be greater than zero.",
                "tank_capacity_invalid");
        }

        return new Tank
        {
            Id = Guid.NewGuid(),
            VesselId = vessel.Id,
            Vessel = vessel,
            Name = trimmedName,
            CapacityCubicMeters = capacityCubicMeters
        };
    }
}

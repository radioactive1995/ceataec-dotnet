using Ceataec.ExampleService.Domain.Vessels.Entities;
using Ceataec.ExampleService.Domain.Vessels.ValueObjects;

namespace Ceataec.ExampleService.Domain.Vessels;

public sealed class Vessel : AggregateRoot
{
    public const int NameMaxLength = 200;
    public const int CreatedByMaxLength = 200;

    private readonly List<Tank> _tanks = [];

    private Vessel()
    {
    }

    public string Name { get; private set; } = string.Empty;
    public ImoNumber ImoNumber { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public IReadOnlyCollection<Tank> Tanks => _tanks;

    public static Vessel Create(string name, ImoNumber imoNumber, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Vessel name is required.", "vessel_name_required");
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > NameMaxLength)
        {
            throw new DomainException(
                $"Vessel name must be at most {NameMaxLength} characters.",
                "vessel_name_too_long");
        }

        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new DomainException("CreatedBy is required.", "created_by_required");
        }

        var trimmedCreatedBy = createdBy.Trim();
        if (trimmedCreatedBy.Length > CreatedByMaxLength)
        {
            throw new DomainException(
                $"CreatedBy must be at most {CreatedByMaxLength} characters.",
                "created_by_too_long");
        }

        return new Vessel
        {
            Id = Guid.NewGuid(),
            Name = trimmedName,
            ImoNumber = imoNumber,
            CreatedBy = trimmedCreatedBy,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void AddTank(string name, decimal capacityCubicMeters)
        => _tanks.Add(Tank.Create(this, name, capacityCubicMeters));
}

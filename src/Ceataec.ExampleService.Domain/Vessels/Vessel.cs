using Ceataec.ExampleService.Domain.Vessels.Entities;
using Ceataec.ExampleService.Domain.Vessels.ValueObjects;
using ErrorOr;

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

    public static ErrorOr<Vessel> Create(string name, ImoNumber imoNumber, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("vessel_name_required", "Vessel name is required.");
        }

        var trimmedName = name.Trim();
        if (trimmedName.Length > NameMaxLength)
        {
            return Error.Validation(
                "vessel_name_too_long",
                $"Vessel name must be at most {NameMaxLength} characters.");
        }

        if (string.IsNullOrWhiteSpace(createdBy))
        {
            return Error.Validation("created_by_required", "CreatedBy is required.");
        }

        var trimmedCreatedBy = createdBy.Trim();
        if (trimmedCreatedBy.Length > CreatedByMaxLength)
        {
            return Error.Validation(
                "created_by_too_long",
                $"CreatedBy must be at most {CreatedByMaxLength} characters.");
        }

        return new Vessel
        {
            Name = trimmedName,
            ImoNumber = imoNumber,
            CreatedBy = trimmedCreatedBy,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public ErrorOr<Tank> AddTank(string name, decimal capacityCubicMeters)
    {
        var tank = Tank.Create(this, name, capacityCubicMeters);
        if (tank.IsError)
        {
            return tank.Errors;
        }

        _tanks.Add(tank.Value);
        return tank.Value;
    }
}

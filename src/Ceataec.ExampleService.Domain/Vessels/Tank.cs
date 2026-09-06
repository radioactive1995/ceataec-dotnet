namespace Ceataec.ExampleService.Domain.Vessels;

public sealed class Tank
{
    public Guid Id { get; set; }
    public Guid VesselId { get; set; }
    public Vessel Vessel { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public decimal CapacityCubicMeters { get; set; }
}

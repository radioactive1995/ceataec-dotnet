namespace Ceataec.ExampleService.Domain.Voyages;

public sealed class Voyage
{
    public Guid Id { get; set; }
    public Guid VesselId { get; set; }
    public string Destination { get; set; } = string.Empty;
    public DateTimeOffset DepartureAt { get; set; }
}

namespace Ceataec.ExampleService.Domain.Vessels;

public sealed class Vessel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImoNumber { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public ICollection<Tank> Tanks { get; set; } = new List<Tank>();
}

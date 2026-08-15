namespace Ceataec.ExampleService.Domain.Certificates;

public sealed class Certificate
{
    public Guid Id { get; set; }
    public Guid VesselId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTimeOffset IssuedOn { get; set; }
}

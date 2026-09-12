using System.ComponentModel.DataAnnotations;

namespace Ceataec.ExampleService.Infrastructure.Settings;

public sealed record DatabaseSettings
{
    public const string SectionName = "Database";

    [Required]
    public required string ConnectionString { get; init; }

    public string GetEffectiveConnectionString(string? preferredConnectionString)
        => string.IsNullOrWhiteSpace(preferredConnectionString)
            ? ConnectionString
            : preferredConnectionString;
}

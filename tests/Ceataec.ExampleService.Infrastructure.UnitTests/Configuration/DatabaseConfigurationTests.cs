using Ceataec.ExampleService.Infrastructure;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ceataec.ExampleService.Infrastructure.UnitTests.Configuration;

public sealed class DatabaseConfigurationTests
{
    private const string Fallback = "Host=localhost;Database=fallback;Username=test";
    private const string Preferred = "Host=localhost;Database=preferred;Username=test";

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Blank_preferred_connection_uses_the_validated_fallback(string? preferred)
    {
        using var provider = CreateProvider(preferred, Fallback);
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Assert.Equal(Fallback, db.Database.GetConnectionString());
    }

    [Fact]
    public void Nonblank_preferred_connection_takes_precedence()
    {
        using var provider = CreateProvider(Preferred, Fallback);
        using var scope = provider.CreateScope();

        Assert.Equal(Preferred, scope.ServiceProvider.GetRequiredService<AppDbContext>()
            .Database.GetConnectionString());
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", " ")]
    [InlineData(" ", "")]
    public void No_effective_connection_fails_options_validation(string? preferred, string? fallback)
    {
        using var provider = CreateProvider(preferred, fallback);

        Assert.Throws<OptionsValidationException>(() =>
            provider.GetRequiredService<IOptions<DatabaseSettings>>().Value);
    }

    private static ServiceProvider CreateProvider(string? preferred, string? fallback)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:ceataec"] = preferred,
                ["Database:ConnectionString"] = fallback,
            })
            .Build();

        return new ServiceCollection().AddInfrastructure(configuration).BuildServiceProvider();
    }
}

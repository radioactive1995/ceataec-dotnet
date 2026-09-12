using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Infrastructure.Providers;
using Ceataec.ExampleService.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ceataec.ExampleService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services
            .AddOptions<DatabaseSettings>()
            .Bind(configuration.GetSection(DatabaseSettings.SectionName))
            .Validate(
                settings => !string.IsNullOrWhiteSpace(configuration.GetConnectionString("ceataec"))
                    || !string.IsNullOrWhiteSpace(settings.ConnectionString),
                "A database connection string is required. Set ConnectionStrings:ceataec or Database:ConnectionString.")
            .ValidateOnStart();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("ceataec")
                ?? sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<DatabaseSettings>>().Value.ConnectionString;
            options.UseNpgsql(connectionString);
        });
        services.AddScoped<ICommandDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IUserProvider, UserProvider>();
        services.AddSingleton<IHashProvider, HashProvider>();

        return services;
    }
}

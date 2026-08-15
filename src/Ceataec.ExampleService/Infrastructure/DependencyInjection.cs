using Ceataec.ExampleService.Infrastructure.Persistence;
using Ceataec.ExampleService.Infrastructure.Persistence.Certificates;
using Ceataec.ExampleService.Infrastructure.Persistence.Vessels;
using Ceataec.ExampleService.Infrastructure.Persistence.Voyages;
using Ceataec.ExampleService.Infrastructure.Providers;
using Ceataec.ExampleService.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;

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
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var database = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<DatabaseSettings>>().Value;
            options.UseNpgsql(database.ConnectionString);
        });

        services.AddScoped<IUserProvider, UserProvider>();
        services.AddSingleton<IHashProvider, HashProvider>();

        services.AddScoped<IVesselRepository, VesselRepository>();
        services.AddScoped<IVoyageRepository, VoyageRepository>();
        services.AddScoped<ICertificateRepository, CertificateRepository>();

        return services;
    }
}

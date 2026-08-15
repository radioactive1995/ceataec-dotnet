using Ceataec.ExampleService.Api.ExceptionHandling;
using Ceataec.ExampleService.Api.Processors;
using FastEndpoints;

namespace Ceataec.ExampleService.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddFastEndpoints();
        services.AddSingleton<AuditPreProcessor>();
        services.AddSingleton<AuditPostProcessor>();

        return services;
    }
}

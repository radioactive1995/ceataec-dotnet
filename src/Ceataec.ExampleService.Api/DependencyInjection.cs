using Ceataec.ExampleService.Api.ExceptionHandling;
using Ceataec.ExampleService.Api.Processors;
using FastEndpoints;
using FastEndpoints.Swagger;

namespace Ceataec.ExampleService.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddFastEndpoints();
        services.SwaggerDocument(o =>
        {
            o.MaxEndpointVersion = 1;
            o.DocumentSettings = s =>
            {
                s.DocumentName = "Release 1";
                s.Title = "Ceataec Example Service";
                s.Version = "v1";
            };
        });
        services.SwaggerDocument(o =>
        {
            o.MaxEndpointVersion = 2;
            o.DocumentSettings = s =>
            {
                s.DocumentName = "Release 2";
                s.Title = "Ceataec Example Service";
                s.Version = "v2";
            };
        });
        services.AddSingleton<AuditPreProcessor>();
        services.AddSingleton<AuditPostProcessor>();

        return services;
    }
}

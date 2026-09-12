using Ceataec.ExampleService.Api.Middleware;
using Ceataec.ExampleService.Api.Processors;
using FastEndpoints;
using FastEndpoints.Swagger;

namespace Ceataec.ExampleService.Api;

public static class WebApplicationExtensions
{
    public static WebApplication UseApi(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseMiddleware<SampleMiddleware>();
        app.UseFastEndpoints(c =>
        {
            c.Errors.UseProblemDetails();
            c.Versioning.Prefix = "v";
            c.Versioning.DefaultVersion = 1;
            c.Versioning.PrependToRoute = true;
            c.Endpoints.Configurator = ep =>
            {
                ep.PreProcessor<AuditPreProcessor>(Order.Before);
                ep.PostProcessor<AuditPostProcessor>(Order.After);
            };
        });
        app.UseSwaggerGen();

        return app;
    }
}

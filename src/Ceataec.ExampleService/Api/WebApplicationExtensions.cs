using Ceataec.ExampleService.Api.Middleware;
using Ceataec.ExampleService.Api.Processors;
using FastEndpoints;

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
            c.Endpoints.Configurator = ep =>
            {
                ep.PreProcessor<AuditPreProcessor>(Order.Before);
                ep.PostProcessor<AuditPostProcessor>(Order.After);
            };
        });

        return app;
    }
}

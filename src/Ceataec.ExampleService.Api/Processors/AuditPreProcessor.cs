using FastEndpoints;

namespace Ceataec.ExampleService.Api.Processors;

public sealed class AuditPreProcessor(ILogger<AuditPreProcessor> logger) : IGlobalPreProcessor
{
    public Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        logger.LogInformation(
            "Handling {Method} {Path}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path);

        return Task.CompletedTask;
    }
}

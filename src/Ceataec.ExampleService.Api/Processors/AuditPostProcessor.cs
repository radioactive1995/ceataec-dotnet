using Ceataec.ExampleService.Infrastructure.Providers;
using FastEndpoints;

namespace Ceataec.ExampleService.Api.Processors;

public sealed class AuditPostProcessor(
    IHashProvider hashProvider,
    ILogger<AuditPostProcessor> logger) : IGlobalPostProcessor
{
    public Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        // Scoped lifetime services can be resolved from the RequestServices property of the HttpContext
        var userProvider = context.HttpContext.RequestServices.GetService<IUserProvider>();
       
        var userId = userProvider?.GetCurrentUserId() ?? "anonymous";
        var userHash = hashProvider.Hash(userId);

        logger.LogInformation(
            "Handled {Method} {Path} for user {UserHash} with status {StatusCode}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path,
            userHash,
            context.HttpContext.Response.StatusCode);

        return Task.CompletedTask;
    }
}

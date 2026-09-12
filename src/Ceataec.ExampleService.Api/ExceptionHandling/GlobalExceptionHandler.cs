using System.Diagnostics;
using Ceataec.ExampleService.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Ceataec.ExampleService.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is DomainException domainException)
        {
            logger.LogWarning(
                domainException,
                "Domain rule violated for {Method} {Path}: {Code}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                domainException.Code);

            var badRequest = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Domain rule violated.",
                Detail = domainException.Message,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
                }
            };

            if (domainException.Code is not null)
            {
                badRequest.Extensions["code"] = domainException.Code;
            }

            httpContext.Response.StatusCode = badRequest.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(badRequest, cancellationToken);
            return true;
        }

        logger.LogError(
            exception,
            "Unhandled exception for {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = "See the application log for details.",
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
            }
        };

        httpContext.Response.StatusCode = problem.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}

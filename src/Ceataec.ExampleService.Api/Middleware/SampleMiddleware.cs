namespace Ceataec.ExampleService.Api.Middleware;

/// <summary>
/// Sample ASP.NET middleware placeholder. Put non-FastEndpoints cross-cutting concerns here
/// (e.g. security headers, path rewrites). Audit/request logging belongs in GlobalPre/GlobalPost processors.
/// </summary>
public sealed class SampleMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context) => next(context);
}

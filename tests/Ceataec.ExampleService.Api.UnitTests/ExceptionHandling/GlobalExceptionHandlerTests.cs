using System.Text.Json;
using Ceataec.ExampleService.Api.ExceptionHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ceataec.ExampleService.Api.UnitTests.ExceptionHandling;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task Unexpected_exception_is_returned_as_internal_server_error()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/vessels";
        context.Response.Body = new MemoryStream();
        var sut = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

        var handled = await sut.TryHandleAsync(
            context,
            new InvalidOperationException("boom"),
            CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        context.Response.Body.Position = 0;
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            context.Response.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.NotNull(problem);
        Assert.Equal("An unexpected error occurred.", problem.Title);
        Assert.Equal("See the application log for details.", problem.Detail);
    }
}

using System.Text.Json;
using Ceataec.ExampleService.Api.ExceptionHandling;
using Ceataec.ExampleService.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ceataec.ExampleService.Api.UnitTests.ExceptionHandling;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task Domain_exception_is_returned_as_bad_request_with_code()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/vessels";
        context.Response.Body = new MemoryStream();
        var exception = new DomainException("Vessel name is required.", "vessel_name_required");
        var sut = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

        var handled = await sut.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        context.Response.Body.Position = 0;
        var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            context.Response.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.NotNull(problem);
        Assert.Equal("Domain rule violated.", problem.Title);
        Assert.Equal("Vessel name is required.", problem.Detail);
        var code = Assert.IsType<JsonElement>(problem.Extensions["code"]);
        Assert.Equal("vessel_name_required", code.GetString());
    }
}

using System.Text.Json;
using ErrorOr;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Ceataec.ExampleService.Http;

internal static class ProblemDetailsMapper
{
    public static ProblemDetails FromDomainErrors(List<Error> errors)
        => Create(
            errors.Select(error => (error.Code, error.Description)).ToList(),
            StatusCodeFor(errors[0].Type));

    public static ProblemDetails FromValidationFailures(
        List<ValidationFailure> failures,
        int statusCode,
        HttpContext httpContext)
        => Create(
            failures.Select(ToCodeAndDescription).ToList(),
            statusCode,
            httpContext.Request.Path);

    private static ProblemDetails Create(
        IReadOnlyList<(string Code, string Description)> errors,
        int statusCode,
        string? instance = null)
    {
        var first = errors[0];
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = ReasonPhrases.GetReasonPhrase(statusCode),
            Detail = first.Description,
            Instance = instance
        };
        problem.Extensions["errors"] = errors
            .Select(error => new { code = error.Code, description = error.Description })
            .ToArray();
        return problem;
    }

    private static (string Code, string Description) ToCodeAndDescription(ValidationFailure failure)
    {
        var code = string.IsNullOrWhiteSpace(failure.PropertyName)
            ? "validation"
            : JsonNamingPolicy.CamelCase.ConvertName(failure.PropertyName);

        return (code, failure.ErrorMessage);
    }

    private static int StatusCodeFor(ErrorType type) => type switch
    {
        ErrorType.Validation or ErrorType.Failure => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.Unexpected => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest
    };
}

using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ceataec.ExampleService.Http;

internal static class ErrorOrExtensions
{
    public static ProblemHttpResult ToProblem(this List<Error> errors)
        => TypedResults.Problem(ProblemDetailsMapper.FromDomainErrors(errors));
}

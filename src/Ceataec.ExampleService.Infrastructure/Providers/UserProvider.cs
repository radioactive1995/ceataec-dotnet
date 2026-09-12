using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Ceataec.ExampleService.Infrastructure.Providers;

public sealed class UserProvider(IHttpContextAccessor httpContextAccessor) : IUserProvider
{
    public string GetCurrentUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? user?.Identity?.Name;

        return string.IsNullOrWhiteSpace(userId) ? "anonymous" : userId;
    }
}

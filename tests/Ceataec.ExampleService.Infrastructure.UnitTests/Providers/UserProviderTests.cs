using System.Security.Claims;
using Ceataec.ExampleService.Infrastructure.Providers;
using Microsoft.AspNetCore.Http;

namespace Ceataec.ExampleService.Infrastructure.UnitTests.Providers;

public sealed class UserProviderTests
{
    [Fact]
    public void GetCurrentUserId_returns_name_identifier_claim()
    {
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, "user-42")
                ], "test"))
            }
        };

        var sut = new UserProvider(accessor);

        Assert.Equal("user-42", sut.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_throws_when_user_missing()
    {
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };

        var sut = new UserProvider(accessor);

        Assert.Throws<InvalidOperationException>(() => sut.GetCurrentUserId());
    }
}

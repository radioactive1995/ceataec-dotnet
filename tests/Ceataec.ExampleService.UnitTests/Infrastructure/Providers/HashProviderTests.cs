using Ceataec.ExampleService.Infrastructure.Providers;

namespace Ceataec.ExampleService.UnitTests.Infrastructure.Providers;

public sealed class HashProviderTests
{
    private readonly HashProvider _sut = new();

    [Fact]
    public void Hash_is_deterministic_for_same_input()
    {
        const string input = "user-42";

        Assert.Equal(_sut.Hash(input), _sut.Hash(input));
    }

    [Fact]
    public void Hash_differs_for_different_inputs()
    {
        Assert.NotEqual(_sut.Hash("user-a"), _sut.Hash("user-b"));
    }
}

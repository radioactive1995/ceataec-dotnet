using NetArchTest.Rules;

namespace Ceataec.ExampleService.ArchitectureTests;

public sealed class LayerTests
{
    [Fact]
    public void Domain_does_not_reference_Infrastructure_Api_or_FastEndpoints()
    {
        Assert.True(
            Types.InAssembly(TestAssemblies.Domain)
                .ShouldNot()
                .HaveDependencyOn("Ceataec.ExampleService.Infrastructure")
                .GetResult()
                .IsSuccessful,
            "Domain must not depend on Infrastructure.");

        Assert.True(
            Types.InAssembly(TestAssemblies.Domain)
                .ShouldNot()
                .HaveDependencyOn("Ceataec.ExampleService.Api")
                .GetResult()
                .IsSuccessful,
            "Domain must not depend on Api.");

        Assert.True(
            Types.InAssembly(TestAssemblies.Domain)
                .ShouldNot()
                .HaveDependencyOn("FastEndpoints")
                .GetResult()
                .IsSuccessful,
            "Domain must not depend on FastEndpoints.");
    }

    [Fact]
    public void Infrastructure_does_not_reference_Api_Features_or_FastEndpoints()
    {
        Assert.True(
            Types.InAssembly(TestAssemblies.Infrastructure)
                .ShouldNot()
                .HaveDependencyOn("Ceataec.ExampleService.Api")
                .GetResult()
                .IsSuccessful,
            "Infrastructure must not depend on Api.");

        Assert.True(
            Types.InAssembly(TestAssemblies.Infrastructure)
                .ShouldNot()
                .HaveDependencyOn("Ceataec.ExampleService.Features")
                .GetResult()
                .IsSuccessful,
            "Infrastructure must not depend on Features.");

        Assert.True(
            Types.InAssembly(TestAssemblies.Infrastructure)
                .ShouldNot()
                .HaveDependencyOn("FastEndpoints")
                .GetResult()
                .IsSuccessful,
            "Infrastructure must not depend on FastEndpoints.");
    }

    [Fact]
    public void No_Host_or_Common_namespaces()
    {
        Assert.Empty(
            Types.InAssemblies([TestAssemblies.Api, TestAssemblies.Domain, TestAssemblies.Infrastructure])
                .That()
                .ResideInNamespaceStartingWith("Ceataec.ExampleService.Host")
                .Or()
                .ResideInNamespaceStartingWith("Ceataec.ExampleService.Common")
                .GetTypes());
    }
}

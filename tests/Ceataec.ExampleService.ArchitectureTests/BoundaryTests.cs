using Ceataec.ExampleService.Infrastructure.Persistence;
using FastEndpoints;
using NetArchTest.Rules;

namespace Ceataec.ExampleService.ArchitectureTests;

public sealed class BoundaryTests
{
    private static readonly System.Reflection.Assembly ApiAssembly = typeof(Program).Assembly;

    [Fact]
    public void Bounded_contexts_are_not_nested()
    {
        Assert.Empty(
            Types.InAssembly(ApiAssembly)
                .That()
                .ResideInNamespaceStartingWith("Ceataec.ExampleService.Domain.Vessels.Voyages")
                .Or()
                .ResideInNamespaceStartingWith("Ceataec.ExampleService.Domain.Vessels.Certificates")
                .Or()
                .ResideInNamespaceStartingWith("Ceataec.ExampleService.Features.Vessels.Voyages")
                .Or()
                .ResideInNamespaceStartingWith("Ceataec.ExampleService.Features.Vessels.Certificates")
                .GetTypes());
    }

    [Fact]
    public void Features_do_not_contain_Persistence_or_Domain_folders()
    {
        Assert.Empty(
            Types.InAssembly(ApiAssembly)
                .That()
                .ResideInNamespaceStartingWith("Ceataec.ExampleService.Features")
                .And()
                .ResideInNamespaceContaining(".Persistence")
                .Or()
                .ResideInNamespaceStartingWith("Ceataec.ExampleService.Features")
                .And()
                .ResideInNamespaceContaining(".Domain")
                .GetTypes());
    }

    [Fact]
    public void Voyages_Persistence_does_not_reference_Vessels_Domain()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .ResideInNamespace("Ceataec.ExampleService.Infrastructure.Persistence.Voyages")
            .ShouldNot()
            .HaveDependencyOn("Ceataec.ExampleService.Domain.Vessels")
            .GetResult();

        Assert.True(result.IsSuccessful, Format(result));
    }

    [Fact]
    public void Certificates_Persistence_does_not_reference_Vessels_Domain()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .ResideInNamespace("Ceataec.ExampleService.Infrastructure.Persistence.Certificates")
            .ShouldNot()
            .HaveDependencyOn("Ceataec.ExampleService.Domain.Vessels")
            .GetResult();

        Assert.True(result.IsSuccessful, Format(result));
    }

    [Fact]
    public void Features_do_not_reference_AppDbContext()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .ResideInNamespaceStartingWith("Ceataec.ExampleService.Features")
            .ShouldNot()
            .HaveDependencyOn(typeof(AppDbContext).FullName!)
            .GetResult();

        Assert.True(result.IsSuccessful, Format(result));
    }

    [Fact]
    public void Api_does_not_contain_endpoints()
    {
        var endpointTypes = Types.InAssembly(ApiAssembly)
            .That()
            .ResideInNamespaceStartingWith("Ceataec.ExampleService.Api")
            .GetTypes()
            .Where(t => IsEndpoint(t))
            .ToList();

        Assert.Empty(endpointTypes);
    }

    [Fact]
    public void No_Host_or_Common_namespaces()
    {
        Assert.Empty(
            Types.InAssembly(ApiAssembly)
                .That()
                .ResideInNamespaceStartingWith("Ceataec.ExampleService.Host")
                .Or()
                .ResideInNamespaceStartingWith("Ceataec.ExampleService.Common")
                .GetTypes());
    }

    [Fact]
    public void Domain_does_not_reference_Persistence_or_FastEndpoints()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .ResideInNamespaceStartingWith("Ceataec.ExampleService.Domain")
            .ShouldNot()
            .HaveDependencyOn("FastEndpoints")
            .GetResult();

        Assert.True(result.IsSuccessful, Format(result));

        Assert.DoesNotContain(
            Types.InAssembly(ApiAssembly)
                .That()
                .ResideInNamespaceStartingWith("Ceataec.ExampleService.Domain")
                .GetTypes(),
            t => t.GetConstructors().Any(c =>
                c.GetParameters().Any(p =>
                    p.ParameterType.Namespace?.Contains(".Persistence") == true)));
    }

    private static bool IsEndpoint(Type type)
        => type.BaseType is { IsGenericType: true } baseType
           && (baseType.GetGenericTypeDefinition() == typeof(Endpoint<,>)
               || baseType.GetGenericTypeDefinition().Name.StartsWith("EndpointWithoutRequest", StringComparison.Ordinal));

    private static string Format(TestResult result)
        => result.FailingTypes is null
            ? "Architecture rule failed."
            : "Architecture rule failed: " + string.Join(", ", result.FailingTypes.Select(t => t.FullName));
}

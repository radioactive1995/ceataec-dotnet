using NetArchTest.Rules;

namespace Ceataec.ExampleService.ArchitectureTests;

public sealed class FeatureTests
{
    [Fact]
    public void Features_do_not_contain_Persistence_or_Domain_folders()
    {
        Assert.Empty(
            Types.InAssembly(TestAssemblies.Api)
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
    public void Endpoints_do_not_reference_AppDbContext()
    {
        var failing = Types.InAssembly(TestAssemblies.Api)
            .That()
            .ResideInNamespaceStartingWith("Ceataec.ExampleService.Features")
            .GetTypes()
            .Where(TestAssemblies.IsEndpoint)
            .Where(TestAssemblies.DependsOnAppDbContext)
            .Select(t => t.FullName)
            .ToList();

        Assert.True(
            failing.Count == 0,
            "Endpoints must not reference AppDbContext: " + string.Join(", ", failing));
    }

    [Fact]
    public void Command_handlers_do_not_reference_AppDbContext()
    {
        var failing = Types.InAssembly(TestAssemblies.Api)
            .That()
            .ResideInNamespaceStartingWith("Ceataec.ExampleService.Features")
            .GetTypes()
            .Where(TestAssemblies.IsCommandHandler)
            .Where(TestAssemblies.DependsOnAppDbContext)
            .Select(t => t.FullName)
            .ToList();

        Assert.True(
            failing.Count == 0,
            "Command handlers must use ICommandDbContext: " + string.Join(", ", failing));
    }

    [Fact]
    public void Api_pipeline_does_not_contain_endpoints()
    {
        var endpointTypes = Types.InAssembly(TestAssemblies.Api)
            .That()
            .ResideInNamespaceStartingWith("Ceataec.ExampleService.Api")
            .GetTypes()
            .Where(TestAssemblies.IsEndpoint)
            .ToList();

        Assert.Empty(endpointTypes);
    }

    [Fact]
    public void Feature_commands_implement_ICommand()
    {
        var failing = Types.InAssembly(TestAssemblies.Api)
            .That()
            .ResideInNamespaceStartingWith("Ceataec.ExampleService.Features")
            .GetTypes()
            .Where(t => t.IsClass && t.Name.EndsWith("Command", StringComparison.Ordinal))
            .Where(t => !TestAssemblies.ImplementsCommandContract(t))
            .Select(t => t.FullName)
            .ToList();

        Assert.True(
            failing.Count == 0,
            "Feature *Command types must implement ICommand / ICommand<>: " + string.Join(", ", failing));
    }

    [Fact]
    public void Feature_queries_implement_IQuery()
    {
        var failing = Types.InAssembly(TestAssemblies.Api)
            .That()
            .ResideInNamespaceStartingWith("Ceataec.ExampleService.Features")
            .GetTypes()
            .Where(t => t.IsClass && t.Name.EndsWith("Query", StringComparison.Ordinal))
            .Where(t => !TestAssemblies.ImplementsQueryContract(t))
            .Select(t => t.FullName)
            .ToList();

        Assert.True(
            failing.Count == 0,
            "Feature *Query types must implement IQuery<>: " + string.Join(", ", failing));
    }

    [Fact]
    public void Command_and_query_handlers_are_not_endpoints()
    {
        var failing = Types.InAssembly(TestAssemblies.Api)
            .That()
            .ResideInNamespaceStartingWith("Ceataec.ExampleService.Features")
            .GetTypes()
            .Where(TestAssemblies.IsCommandOrQueryHandler)
            .Where(TestAssemblies.IsEndpoint)
            .Select(t => t.FullName)
            .ToList();

        Assert.True(
            failing.Count == 0,
            "Handlers must not be endpoints: " + string.Join(", ", failing));
    }

    [Fact]
    public void Endpoints_do_not_implement_command_or_query_handlers()
    {
        var failing = Types.InAssembly(TestAssemblies.Api)
            .That()
            .ResideInNamespaceStartingWith("Ceataec.ExampleService.Features")
            .GetTypes()
            .Where(TestAssemblies.IsEndpoint)
            .Where(TestAssemblies.IsCommandOrQueryHandler)
            .Select(t => t.FullName)
            .ToList();

        Assert.True(
            failing.Count == 0,
            "Endpoints must not implement handler interfaces: " + string.Join(", ", failing));
    }

    [Fact]
    public void Cqrs_namespace_contains_only_marker_interfaces()
    {
        var types = Types.InAssembly(TestAssemblies.Api)
            .That()
            .ResideInNamespace("Ceataec.ExampleService.Cqrs")
            .GetTypes()
            .Where(t => t.IsPublic)
            .ToList();

        Assert.All(types, t => Assert.True(t.IsInterface, $"{t.FullName} must be an interface."));

        var names = types.Select(t => t.Name).OrderBy(n => n).ToArray();
        Assert.Equal(["IQuery`1", "IQueryHandler`2"], names);
    }
}

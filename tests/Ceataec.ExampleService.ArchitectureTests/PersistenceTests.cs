using Ceataec.ExampleService.Domain;
using Ceataec.ExampleService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NetArchTest.Rules;

namespace Ceataec.ExampleService.ArchitectureTests;

public sealed class PersistenceTests
{
    private static readonly string PersistenceRoot = FindPersistenceRoot();

    [Fact]
    public void Aggregate_context_Set_is_constrained_to_aggregate_roots()
    {
        var setMethod = typeof(ICommandDbContext)
            .GetMethods()
            .Single(m => m.Name == nameof(ICommandDbContext.Set));
        var aggregateType = Assert.Single(setMethod.GetGenericArguments());

        Assert.Contains(typeof(AggregateRoot), aggregateType.GetGenericParameterConstraints());
        Assert.True(setMethod.ReturnType.IsGenericType);
        Assert.Equal(typeof(DbSet<>), setMethod.ReturnType.GetGenericTypeDefinition());
        Assert.Same(aggregateType, Assert.Single(setMethod.ReturnType.GetGenericArguments()));
    }

    [Fact]
    public void Persistence_query_types_implement_IDbQuery()
    {
        var queryTypes = Types.InAssembly(TestAssemblies.Infrastructure)
            .GetTypes()
            .Where(t => t.IsPublic && IsPersistenceQueryType(t))
            .ToList();

        Assert.NotEmpty(queryTypes);

        var failing = queryTypes
            .Where(t => !ImplementsIDbQuery(t))
            .Select(t => t.FullName)
            .ToList();

        Assert.True(
            failing.Count == 0,
            "Persistence query types must implement IDbQuery<,>: " + string.Join(", ", failing));
    }

    [Fact]
    public void IDbQuery_QueryAsync_uses_AsNoTracking()
    {
        var queryTypes = Types.InAssembly(TestAssemblies.Infrastructure)
            .GetTypes()
            .Where(t => t.IsPublic && ImplementsIDbQuery(t))
            .ToList();

        Assert.NotEmpty(queryTypes);
        Assert.True(Directory.Exists(PersistenceRoot), $"Persistence root not found: {PersistenceRoot}");

        var failing = new List<string>();

        foreach (var type in queryTypes)
        {
            var sourcePath = FindSourceFile(type.Name);
            if (sourcePath is null)
            {
                failing.Add($"{type.FullName} (source file not found under Persistence)");
                continue;
            }

            var source = File.ReadAllText(sourcePath);
            if (!source.Contains(".AsNoTracking(", StringComparison.Ordinal))
            {
                failing.Add($"{type.FullName} ({sourcePath})");
            }
        }

        Assert.True(
            failing.Count == 0,
            "IDbQuery QueryAsync must use AsNoTracking: " + string.Join(", ", failing));
    }

    private static bool IsPersistenceQueryType(Type type)
        => IsPersistenceQueriesNamespace(type.Namespace);

    private static bool IsPersistenceQueriesNamespace(string? ns)
        => ns is not null
           && ns.StartsWith("Ceataec.ExampleService.Infrastructure.Persistence.", StringComparison.Ordinal)
           && ns.EndsWith(".Queries", StringComparison.Ordinal);

    private static bool ImplementsIDbQuery(Type type)
        => type.GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDbQuery<,>));

    private static string? FindSourceFile(string typeName)
    {
        return Directory
            .EnumerateFiles(PersistenceRoot, $"{typeName}.cs", SearchOption.AllDirectories)
            .FirstOrDefault();
    }

    private static string FindPersistenceRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(
                dir.FullName,
                "src",
                "Ceataec.ExampleService.Infrastructure",
                "Persistence");

            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException(
            "Could not locate src/Ceataec.ExampleService.Infrastructure/Persistence from test BaseDirectory.");
    }
}

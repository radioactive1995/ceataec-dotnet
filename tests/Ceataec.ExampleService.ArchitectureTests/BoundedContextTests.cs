using NetArchTest.Rules;

namespace Ceataec.ExampleService.ArchitectureTests;

public sealed class BoundedContextTests
{
    [Fact]
    public void Bounded_contexts_are_not_nested()
    {
        var forbidden = new List<string>();

        foreach (var outer in TestAssemblies.BoundedContexts)
        {
            foreach (var inner in TestAssemblies.BoundedContexts)
            {
                if (outer == inner)
                {
                    continue;
                }

                forbidden.Add($"Ceataec.ExampleService.Domain.{outer}.{inner}");
                forbidden.Add($"Ceataec.ExampleService.Features.{outer}.{inner}");
            }
        }

        var nested = Types.InAssemblies([TestAssemblies.Api, TestAssemblies.Domain])
            .GetTypes()
            .Where(t => t.Namespace is not null
                        && forbidden.Any(prefix =>
                            t.Namespace.Equals(prefix, StringComparison.Ordinal)
                            || t.Namespace.StartsWith(prefix + ".", StringComparison.Ordinal)))
            .Select(t => t.FullName)
            .ToList();

        Assert.True(
            nested.Count == 0,
            "Bounded contexts must not be nested: " + string.Join(", ", nested));
    }

    [Fact]
    public void Persistence_bc_does_not_reference_other_domain_bcs()
    {
        foreach (var persistenceBc in TestAssemblies.BoundedContexts)
        {
            foreach (var domainBc in TestAssemblies.BoundedContexts)
            {
                if (persistenceBc == domainBc)
                {
                    continue;
                }

                var result = Types.InAssembly(TestAssemblies.Infrastructure)
                    .That()
                    .ResideInNamespace($"Ceataec.ExampleService.Infrastructure.Persistence.{persistenceBc}")
                    .ShouldNot()
                    .HaveDependencyOn($"Ceataec.ExampleService.Domain.{domainBc}")
                    .GetResult();

                Assert.True(
                    result.IsSuccessful,
                    $"Persistence.{persistenceBc} must not depend on Domain.{domainBc}. "
                    + TestAssemblies.Format(result));
            }
        }
    }
}

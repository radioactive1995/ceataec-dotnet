using Ceataec.ExampleService.Domain.Vessels;
using Ceataec.ExampleService.Infrastructure.Persistence;
using FastEndpoints;
using NetArchTest.Rules;

namespace Ceataec.ExampleService.ArchitectureTests;

internal static class TestAssemblies
{
    public static readonly System.Reflection.Assembly Api = typeof(Program).Assembly;
    public static readonly System.Reflection.Assembly Domain = typeof(Vessel).Assembly;
    public static readonly System.Reflection.Assembly Infrastructure = typeof(AppDbContext).Assembly;

    public static bool IsEndpoint(Type type)
        => type.BaseType is { IsGenericType: true } baseType
           && (baseType.GetGenericTypeDefinition() == typeof(Endpoint<,>)
               || baseType.GetGenericTypeDefinition().Name.StartsWith("EndpointWithoutRequest", StringComparison.Ordinal));

    public static bool ImplementsOpenGeneric(Type type, Type openGeneric)
        => type.GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric);

    public static bool ImplementsCommandContract(Type type)
        => typeof(ICommand).IsAssignableFrom(type)
           || ImplementsOpenGeneric(type, typeof(ICommand<>));

    public static bool ImplementsQueryContract(Type type)
        => ImplementsOpenGeneric(type, typeof(Ceataec.ExampleService.Cqrs.IQuery<>));

    public static bool IsCommandOrQueryHandler(Type type)
        => type.GetInterfaces().Any(i =>
            i.IsGenericType
            && (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
                || i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)
                || i.GetGenericTypeDefinition() == typeof(Ceataec.ExampleService.Cqrs.IQueryHandler<,>)));

    public static bool IsCommandHandler(Type type)
        => !ImplementsOpenGeneric(
               type,
               typeof(Ceataec.ExampleService.Cqrs.IQueryHandler<,>))
           && type.GetInterfaces().Any(i =>
               i.IsGenericType
               && (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
                   || i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));

    public static bool DependsOnAppDbContext(Type type)
    {
        if (type.GetConstructors().Any(c =>
                c.GetParameters().Any(p => p.ParameterType == typeof(AppDbContext))))
        {
            return true;
        }

        const System.Reflection.BindingFlags flags =
            System.Reflection.BindingFlags.Instance
            | System.Reflection.BindingFlags.Public
            | System.Reflection.BindingFlags.NonPublic
            | System.Reflection.BindingFlags.DeclaredOnly;

        return type.GetMethods(flags)
                   .SelectMany(m => m.GetParameters())
                   .Any(p => p.ParameterType == typeof(AppDbContext))
               || type.GetFields(flags).Any(f => f.FieldType == typeof(AppDbContext))
               || type.GetProperties(flags).Any(p => p.PropertyType == typeof(AppDbContext));
    }

    public static string Format(TestResult result)
        => result.FailingTypes is null
            ? "Architecture rule failed."
            : "Architecture rule failed: " + string.Join(", ", result.FailingTypes.Select(t => t.FullName));
}

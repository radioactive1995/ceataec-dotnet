namespace Ceataec.ExampleService.Api.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection : ICollectionFixture<ExampleWebApplicationFactory>
{
    public const string Name = "Integration";
}

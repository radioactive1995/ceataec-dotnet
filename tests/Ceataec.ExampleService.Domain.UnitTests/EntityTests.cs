using Ceataec.ExampleService.Domain;

namespace Ceataec.ExampleService.Domain.UnitTests;

public sealed class EntityTests
{
    [Fact]
    public void Same_instance_is_equal_while_transient()
    {
        var entity = new TestEntity();

        Assert.Equal(entity, entity);
    }

    [Fact]
    public void Different_transient_entities_are_not_equal()
    {
        Assert.NotEqual(new TestEntity(), new TestEntity());
    }

    [Fact]
    public void Same_type_and_identity_are_equal()
    {
        var id = Guid.NewGuid();

        Assert.Equal(new TestEntity(id), new TestEntity(id));
    }

    [Fact]
    public void Different_entity_types_are_not_equal()
    {
        var id = Guid.NewGuid();

        Assert.NotEqual<Entity>(new TestEntity(id), new OtherTestEntity(id));
    }

    private sealed class TestEntity : Entity
    {
        public TestEntity()
        {
        }

        public TestEntity(Guid id) => Id = id;
    }

    private sealed class OtherTestEntity : Entity
    {
        public OtherTestEntity(Guid id) => Id = id;
    }
}

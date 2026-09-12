namespace Ceataec.ExampleService.Domain;

/// <summary>
/// Represents the root entity that controls an aggregate's consistency boundary.
/// </summary>
public abstract class AggregateRoot : Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
    /// </summary>
    protected AggregateRoot()
    {
    }
}

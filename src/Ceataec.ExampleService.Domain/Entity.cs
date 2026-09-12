namespace Ceataec.ExampleService.Domain;

/// <summary>
/// Represents a domain object identified by a stable identity.
/// </summary>
public abstract class Entity : IEquatable<Entity>
{
    /// <summary>
    /// Gets the identity of the entity.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    protected Entity()
    {
    }

    /// <summary>
    /// Determines whether this entity and another entity have the same type and non-empty identity.
    /// </summary>
    /// <param name="other">
    /// The entity to compare with this entity.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when the entities are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(Entity? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return Id != Guid.Empty && Id == other.Id;
    }

    /// <summary>
    /// Determines whether this entity and an object have the same type and non-empty identity.
    /// </summary>
    /// <param name="obj">
    /// The object to compare with this entity.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when the objects are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(object? obj) => Equals(obj as Entity);

    /// <summary>
    /// Returns the hash code for this entity's identity.
    /// </summary>
    /// <returns>
    /// The hash code for the entity identity.
    /// </returns>
    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Determines whether two entities are equal.
    /// </summary>
    /// <param name="left">
    /// The first entity to compare.
    /// </param>
    /// <param name="right">
    /// The second entity to compare.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when the entities are equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(Entity? left, Entity? right)
        => left is null ? right is null : left.Equals(right);

    /// <summary>
    /// Determines whether two entities are not equal.
    /// </summary>
    /// <param name="left">
    /// The first entity to compare.
    /// </param>
    /// <param name="right">
    /// The second entity to compare.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when the entities are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(Entity? left, Entity? right) => !(left == right);
}

namespace Simple.Domain.Common;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : TypedId
{
    protected Entity(TId id)
    {
        Id = id;
    }

#pragma warning disable CS8618
    protected Entity()
    {
    }
#pragma warning restore CS8618

    public TId Id { get; }

    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?)other);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other) return false;
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !Equals(left, right);
    }
}
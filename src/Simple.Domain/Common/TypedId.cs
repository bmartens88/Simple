namespace Simple.Domain.Common;

public abstract class TypedId<TValue>(TValue value) : TypedId
    where TValue : struct
{
    public TValue Value { get; } = value;

    protected sealed override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

public abstract class TypedId : ValueObject
{
}
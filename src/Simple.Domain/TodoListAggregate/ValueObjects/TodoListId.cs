using Ardalis.GuardClauses;
using Simple.Domain.Common;

namespace Simple.Domain.TodoListAggregate.ValueObjects;

public sealed class TodoListId : TypedId<Guid>
{
    private TodoListId(Guid value)
        : base(Guard.Against.NullOrEmpty(value))
    {
    }

    public static TodoListId Create(Guid value)
    {
        return new TodoListId(value);
    }

    public static TodoListId CreateUnique()
    {
        return new TodoListId(Guid.NewGuid());
    }
}
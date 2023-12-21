using Ardalis.GuardClauses;
using Simple.Domain.Common;

namespace Simple.Domain.TodoListAggregate.ValueObjects;

public sealed class TodoItemId : TypedId<Guid>
{
    private TodoItemId(Guid value)
        : base(Guard.Against.NullOrEmpty(value))
    {
    }

    public static TodoItemId Create(Guid value)
    {
        return new TodoItemId(value);
    }

    public static TodoItemId CreateUnique()
    {
        return new TodoItemId(Guid.NewGuid());
    }
}
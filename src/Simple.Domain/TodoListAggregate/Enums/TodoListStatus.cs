using Ardalis.SmartEnum;

namespace Simple.Domain.TodoListAggregate.Enums;

public abstract class TodoListStatus : SmartEnum<TodoListStatus>
{
    public static readonly TodoListStatus Draft = new DraftStatus(nameof(Draft), 0);
    public static readonly TodoListStatus Published = new PublishedStatus(nameof(Published), 1);
    public static readonly TodoListStatus Completed = new CompletedStatus(nameof(Completed), 2);
    public static readonly TodoListStatus Updated = new UpdatedStatus(nameof(Updated), 3);

    private TodoListStatus(string name, int value)
        : base(name, value)
    {
    }

    public abstract bool CanTransitionTo(TodoListStatus next);

    public sealed class DraftStatus(string name, int value) : TodoListStatus(name, value)
    {
        public override bool CanTransitionTo(TodoListStatus next)
        {
            return next == Published;
        }
    }

    public sealed class PublishedStatus(string name, int value) : TodoListStatus(name, value)
    {
        public override bool CanTransitionTo(TodoListStatus next)
        {
            return next == Completed || next == Draft;
        }
    }

    public sealed class CompletedStatus(string name, int value) : TodoListStatus(name, value)
    {
        public override bool CanTransitionTo(TodoListStatus next)
        {
            return next == Updated;
        }
    }

    public sealed class UpdatedStatus(string name, int value) : TodoListStatus(name, value)
    {
        public override bool CanTransitionTo(TodoListStatus next)
        {
            return next == Completed;
        }
    }
}
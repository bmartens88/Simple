using Ardalis.GuardClauses;
using Simple.Domain.Common;
using Simple.Domain.TodoListAggregate.ValueObjects;

namespace Simple.Domain.TodoListAggregate.Entities;

public sealed class TodoItem : Entity<TodoItemId>
{
    private TodoItem(
        string title,
        string description,
        TodoItemId id)
        : base(id)
    {
        Title = Guard.Against.Length(Guard.Against.NullOrEmpty(title), 100, nameof(title));
        Description = Guard.Against.Length(Guard.Against.NullOrEmpty(description), 200, nameof(description));
    }

    public string Title { get; }

    public string Description { get; }

    public bool Completed { get; private set; }

    public DateTime CreatedOnUtc { get; } = DateTime.UtcNow;

    public DateTime? CompletedOnUtc { get; private set; }

    public static TodoItem Create(
        string title,
        string description,
        TodoItemId? id = null)
    {
        return new TodoItem(title, description, id ?? TodoItemId.CreateUnique());
    }

    internal void CompleteTodoItem()
    {
        if (Completed) return;

        Completed = true;
        CompletedOnUtc = DateTime.UtcNow;
    }
}
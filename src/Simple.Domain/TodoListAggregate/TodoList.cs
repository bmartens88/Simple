using Ardalis.GuardClauses;
using ErrorOr;
using Simple.Domain.Common;
using Simple.Domain.TodoListAggregate.Entities;
using Simple.Domain.TodoListAggregate.Enums;
using Simple.Domain.TodoListAggregate.Errors;
using Simple.Domain.TodoListAggregate.ValueObjects;

namespace Simple.Domain.TodoListAggregate;

public sealed class TodoList : AggregateRoot<TodoListId>
{
    private readonly List<TodoItem> _items = [];

    private TodoList(
        string title,
        string description,
        IEnumerable<TodoItem> items,
        TodoListId id)
        : base(id)
    {
        Title = Guard.Against.Length(Guard.Against.NullOrEmpty(title), 100, nameof(title));
        Description = Guard.Against.Length(Guard.Against.NullOrEmpty(description), 200, nameof(description));
        _items.AddRange(Guard.Against.NullOrEmpty(items));
    }

    public IReadOnlyList<TodoItem> Items => _items.AsReadOnly();

    public string Title { get; }

    public string Description { get; }

    public TodoListStatus Status { get; private set; } = TodoListStatus.Draft;

    public DateTime CreatedOnUtc { get; } = DateTime.UtcNow;

    public DateTime? CompletedOnUtc { get; private set; }

    public static TodoList Create(
        string title,
        string description,
        IEnumerable<TodoItem>? items = null,
        TodoListId? id = null)
    {
        return new TodoList(title, description, items ?? [], id ?? TodoListId.CreateUnique());
    }

    public ErrorOr<Success> AddTodoItem(TodoItem todoItem)
    {
        var item = GetItemById(todoItem.Id);
        if (item is not null)
            return AppErrors.TodoListErrors.ItemAlreadyExists;
        _items.Add(todoItem);
        UpdateState();
        return Result.Success;
    }

    public ErrorOr<Success> RemoveTodoItem(TodoItemId itemId)
    {
        var item = GetItemById(itemId);
        if (item is null)
            return AppErrors.TodoListErrors.ItemNotFound;
        _items.Remove(item);
        UpdateState();
        return Result.Success;
    }

    public ErrorOr<Success> CompleteTodoItem(TodoItemId itemId)
    {
        if (!HasListBeenPublished())
            return AppErrors.TodoListErrors.ListNotYetPublished;
        var item = GetItemById(itemId);
        if (item is null)
            return AppErrors.TodoListErrors.ItemNotFound;
        if (item.Completed)
            return AppErrors.TodoListErrors.ItemAlreadyCompleted;
        item.CompleteTodoItem();
        UpdateState();
        return Result.Success;
    }

    public ErrorOr<Success> PublishTodoList()
    {
        if (HasListBeenPublished())
            return AppErrors.TodoListErrors.ListAlreadyPublished;
        Status = TodoListStatus.Published;
        return Result.Success;
    }

    public ErrorOr<Success> UnpublishTodoList()
    {
        if (!HasListBeenPublished())
            return AppErrors.TodoListErrors.ListNotYetPublished;
        if (!Status.CanTransitionTo(TodoListStatus.Draft))
            return AppErrors.TodoListErrors.InvalidStateTransition;
        Status = TodoListStatus.Draft;
        return Result.Success;
    }

    private TodoItem? GetItemById(TodoItemId itemId)
    {
        return _items.SingleOrDefault(item => item.Id == itemId);
    }

    private bool HasListBeenPublished()
    {
        return Status >= TodoListStatus.Published;
    }

    private TodoListStatus DetermineStatus()
    {
        var allCompleted = _items.TrueForAll(item => item.Completed);

        var newStatus = Status.Name switch
        {
            nameof(TodoListStatus.Published) when allCompleted => TodoListStatus.Completed,
            nameof(TodoListStatus.Completed) when !allCompleted => TodoListStatus.Updated,
            nameof(TodoListStatus.Updated) when allCompleted => TodoListStatus.Completed,
            _ => TodoListStatus.Draft
        };

        return newStatus;
    }

    private void UpdateState()
    {
        var newState = DetermineStatus();

        if (Status == newState) return;

        if (!Status.CanTransitionTo(newState))
        {
            // Perhaps do something else than throwing? Rely more on the .CanTransitionTo?
        }

        Status = newState;
        CompletedOnUtc = Status == TodoListStatus.Completed
            ? DateTime.UtcNow
            : null;
    }
}
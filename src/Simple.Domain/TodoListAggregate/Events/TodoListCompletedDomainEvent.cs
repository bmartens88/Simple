using Simple.Domain.Common.Events;
using Simple.Domain.TodoListAggregate.ValueObjects;

namespace Simple.Domain.TodoListAggregate.Events;

public sealed class TodoListCompletedDomainEvent(TodoList todoList) : BaseDomainEvent
{
    public TodoListId TodoListId { get; } = todoList.Id;
}
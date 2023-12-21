using FluentAssertions;
using Simple.Domain.TodoListAggregate;
using Simple.Domain.TodoListAggregate.Entities;
using Simple.Domain.TodoListAggregate.Enums;
using Simple.Domain.TodoListAggregate.Errors;
using Simple.Domain.TodoListAggregate.ValueObjects;

namespace Simple.Domain.UnitTests.TodoListAggregate;

public sealed class TodoListTests
{
    private const string Title = "A perfectly valid test title";
    private const string Description = "A perfectly valid test description";

    private static TodoItem CreateTodoItem(string title = Title, string description = Description,
        TodoItemId? id = null)
    {
        return TodoItem.Create(title, description, id);
    }

    private static TodoList CreateTodoList(string title = Title, string description = Description,
        IEnumerable<TodoItem>? items = null, TodoListId? id = null)
    {
        return TodoList.Create(title, description, items, id);
    }

    public static IEnumerable<TodoItem> CreateTodoItemList()
    {
        return [CreateTodoItem()];
    }

    [Fact]
    public void Creat_ShouldReturnTodoList_WhenAllParametersAreValid()
    {
        // Arrange
        var items = CreateTodoItemList();

        // Act
        var todoList = CreateTodoList(items: items);

        // Assert
        todoList.Should().NotBeNull();
    }

    [Fact]
    public void Create_ShouldUseTitle_WhenTitleParameterIsValid()
    {
        // Arrange
        var items = CreateTodoItemList();

        // Act
        var todoList = CreateTodoList(items: items);

        // Assert
        todoList.Title.Should().NotBeNull();
        todoList.Title.Should().Be(Title);
    }

    [Fact]
    public void Creat_ShouldUseDescription_WhenDescriptionParameterIsValid()
    {
        // Arrange
        var items = CreateTodoItemList();

        // Act
        var todoList = CreateTodoList(items: items);

        // Assert
        todoList.Description.Should().NotBeNull();
        todoList.Description.Should().Be(Description);
    }

    [Fact]
    public void Create_ShouldCreateId_WhenNoIdIsProvided()
    {
        // Arrange
        var items = CreateTodoItemList();

        // Act
        var todoList = CreateTodoList(items: items);

        // Assert
        todoList.Id.Should().NotBeNull();
    }

    [Fact]
    public void Create_ShouldUseId_WhenIdIsProvided()
    {
        // Arrange
        var items = CreateTodoItemList();
        var guid = Guid.NewGuid();
        var id = TodoListId.Create(guid);

        // Act
        var todoList = CreateTodoList(items: items, id: id);

        // Assert
        todoList.Id.Should().NotBeNull();
        todoList.Id.Value.Should().Be(guid);
    }

    [Fact]
    public void Create_ShouldThrow_WhenNoInitialItemsAreProvided()
    {
        // Act
        var action = () => CreateTodoList();

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName("items");
    }

    [Theory]
    [InlineData(null)]
    [InlineData(
        "A perfectly fine test title in content, but not in length. A title which exceeds the maximum length of 100 characters will throw an exception.")]
    public void Create_ShouldThrow_WhenTitleParameterIsInvalid(string? title)
    {
        // Arrange
        var items = CreateTodoItemList();

        // Act
        var action = () => CreateTodoList(title!, items: items);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName(nameof(title));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(
        "A perfectly fine test description in content, but not in length. A description which exceeds the maximum length of 200 characters will throw an exception. This is covered thanks to the GuardClauses library.")]
    public void Create_ShouldThrow_WhenDescriptionParameterIsInvalid(string? description)
    {
        // Arrange
        var items = CreateTodoItemList();

        // Act
        var action = () => CreateTodoList(description: description!, items: items);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName(nameof(description));
    }

    [Fact]
    public void AddTodoItem_ShouldAddTodoItem_WhenTheItemDoesNotYetExist()
    {
        // Arrange
        var items = CreateTodoItemList();
        var todoList = CreateTodoList(items: items);
        var newItem = CreateTodoItem();

        // Act
        todoList.AddTodoItem(newItem);

        // Assert
        todoList.Items.Count.Should().Be(2);
        todoList.Items[^1].Should().Be(newItem);
    }

    [Fact]
    public void AddTodoItem_ShouldReturnError_WhenItemAlreadyExists()
    {
        // Arrange
        var item = CreateTodoItem();
        var todoList = CreateTodoList(items: [item]);

        // Act
        var result = todoList.AddTodoItem(item);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(AppErrors.TodoListErrors.ItemAlreadyExists);
    }

    [Fact]
    public void AddTodoItem_ShouldUpdateStateFromCompletedToUpdated_WhenTheListWasCompleted()
    {
        // Arrange
        var item = CreateTodoItem();
        var itemId = item.Id;
        var todoList = CreateTodoList(items: [item]);
        todoList.PublishTodoList();
        todoList.CompleteTodoItem(itemId);
        var newItem = CreateTodoItem();

        // Act
        var result = todoList.AddTodoItem(newItem);

        // Assert
        result.IsError.Should().BeFalse();
        todoList.Status.Should().Be(TodoListStatus.Updated);
    }

    [Fact]
    public void RemoveTodoItem_ShouldRemoveTodoItem_WhenItemExists()
    {
        // Arrange
        var item = CreateTodoItem();
        var todoList = CreateTodoList(items: [item]);

        // Act
        var result = todoList.RemoveTodoItem(item.Id);

        // Assert
        result.IsError.Should().BeFalse();
        todoList.Items.Count.Should().Be(0);
    }

    [Fact]
    public void RemoveTodoItem_ShouldReturnError_WhenItemDoesNotExist()
    {
        // Arrange
        var item = CreateTodoItem();
        var todoList = CreateTodoList(items: [item]);

        // Act
        var result = todoList.RemoveTodoItem(TodoItemId.CreateUnique());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(AppErrors.TodoListErrors.ItemNotFound);
    }

    [Fact]
    public void RemoveTodoItem_ShouldUpdateStateFromUpdatedToCompleted_WhenAllItemsAreCompletedAfterRemoval()
    {
        // Arrange
        var item = CreateTodoItem();
        var todoList = CreateTodoList(items: [item]);
        todoList.PublishTodoList();
        todoList.CompleteTodoItem(item.Id);
        var newItem = CreateTodoItem();
        todoList.AddTodoItem(newItem);

        // Act
        var result = todoList.RemoveTodoItem(newItem.Id);

        // Assert
        result.IsError.Should().BeFalse();
        todoList.Status.Should().Be(TodoListStatus.Completed);
    }
}
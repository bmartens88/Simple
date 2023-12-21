using FluentAssertions;
using Simple.Domain.TodoListAggregate.Entities;
using Simple.Domain.TodoListAggregate.ValueObjects;

namespace Simple.Domain.UnitTests.TodoListAggregate.Entities;

public sealed class TodoItemTests
{
    private const string Title = "A perfectly fine test title";
    private const string Description = "A perfectly fine test description";

    private static TodoItem CreateTodoItem(string title = Title, string description = Description,
        TodoItemId? id = null)
    {
        return TodoItem.Create(title, description, id);
    }

    [Fact]
    public void Create_ShouldReturnTodoItem_WhenAllParametersAreValid()
    {
        // Act
        var todoItem = CreateTodoItem();

        // Assert
        todoItem.Should().NotBeNull();
    }

    [Fact]
    public void Create_ShouldUseTitle_WhenTitleParameterIsValid()
    {
        // Act
        var todoItem = CreateTodoItem();
        
        // Assert
        todoItem.Title.Should().Be(Title);
    }

    [Fact]
    public void Create_ShouldUseDescription_WhenDescriptionParameterIsValid()
    {
        // Act
        var todoItem = CreateTodoItem();
        
        // Assert
        todoItem.Description.Should().Be(Description);
    }

    [Fact]
    public void Create_ShouldGenerateId_WhenNoIdIsProvided()
    {
        // Act
        var todoItem = CreateTodoItem();
        
        // Assert
        todoItem.Id.Should().NotBeNull();
    }

    [Fact]
    public void Create_ShouldUseId_WhenIdIsProvided()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var id = TodoItemId.Create(guid);
        
        // Act
        var todoItem = CreateTodoItem(id: id);
        
        // Assert
        todoItem.Id.Should().Be(id);
        todoItem.Id.Value.Should().Be(guid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(
        "A perfectly fine test title in content, but not in length. A title which exceeds the maximum length of 100 characters will throw an exception.")]
    public void Create_ShouldThrow_WhenTitleParameterIsInvalid(string? title)
    {
        // Act
        var action = () => CreateTodoItem(title!);

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
        // Act
        var action = () => CreateTodoItem(description: description!);

        // Assert
        action.Should().Throw<ArgumentException>()
            .WithParameterName(nameof(description));
    }

    [Fact]
    public void CompleteTodoItem_ShouldCompleteTodoItem_WhenItemIsNotYetCompleted()
    {
        // Arrange
        var todoItem = CreateTodoItem();
        
        // Act
        todoItem.CompleteTodoItem();
        
        // Assert
        todoItem.Completed.Should().BeTrue();
        todoItem.CompletedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(250));
    }

    [Fact]
    public async void CompleteTodoItem_ShouldReturn_WhenItemIsAlreadyCompleted()
    {
        // Arrange
        var todoItem = CreateTodoItem();
        var utcNow = DateTime.UtcNow;
        todoItem.CompleteTodoItem();
        
        // Act
        await Task.Delay(1500);
        todoItem.CompleteTodoItem();
        
        // Assert
        todoItem.Completed.Should().BeTrue();
        // Even though we waited for a short period of time, the CompletedOnUtc should not have been updated.
        todoItem.CompletedOnUtc.Should().BeCloseTo(utcNow, TimeSpan.FromMilliseconds(250));
    }
}
using ErrorOr;

namespace Simple.Domain.TodoListAggregate.Errors;

public static class AppErrors
{
    public static class TodoListErrors
    {
        public static readonly Error ItemAlreadyExists = Error.Conflict(
            $"TodoListErrors.{nameof(ItemAlreadyExists)}",
            "Can't add the same item twice.");

        public static readonly Error ItemNotFound = Error.NotFound(
            $"TodoListErrors.{nameof(ItemNotFound)}", 
            "The specified item was not found.");

        public static readonly Error ItemAlreadyCompleted = Error.Conflict(
            $"TodoListErrors.{nameof(ItemAlreadyCompleted)}",
            "A completed item can't be completed again.");

        public static readonly Error ListNotYetPublished = Error.Conflict(
            $"TodoListErrors.{nameof(ListNotYetPublished)}",
            "The list has not yet been published.");

        public static readonly Error ListAlreadyPublished = Error.Conflict(
            $"TodoListErrors.{nameof(ListAlreadyPublished)}",
            "Can't publish an already published list.");

        public static readonly Error InvalidStateTransition = Error.Failure(
            $"TodoListErrors.{nameof(InvalidStateTransition)}",
            "Trying to perform an invalid state transition.");
    }
}
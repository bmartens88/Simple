using MediatR;

namespace Simple.Domain.Common.Interfaces;

public interface IDomainEvent : INotification
{
    DateTime OccurredOnUtc { get; }
}
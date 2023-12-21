using Simple.Domain.Common.Interfaces;

namespace Simple.Domain.Common.Events;

public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
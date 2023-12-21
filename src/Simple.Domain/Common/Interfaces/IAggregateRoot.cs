namespace Simple.Domain.Common.Interfaces;

public interface IAggregateRoot
{
    IReadOnlyList<IDomainEvent> PopDomainEvents();
}
using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Application.Common.Events;

public interface IDomainEventsDispatcher
{
    Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default);
}

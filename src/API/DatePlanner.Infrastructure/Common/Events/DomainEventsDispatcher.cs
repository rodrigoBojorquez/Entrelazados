using DatePlanner.Application.Common.Events;
using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Infrastructure.Common.Events;

internal sealed class DomainEventsDispatcher(IPublisher publisher) : IDomainEventsDispatcher
{
    public async Task DispatchAsync(
        IReadOnlyCollection<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            Type notificationType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());
            object notification = Activator.CreateInstance(notificationType, domainEvent)!;

            await publisher.Publish(notification, cancellationToken);
        }
    }
}

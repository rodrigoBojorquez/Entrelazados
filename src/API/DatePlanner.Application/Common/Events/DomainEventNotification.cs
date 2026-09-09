using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Common.Events;

public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;

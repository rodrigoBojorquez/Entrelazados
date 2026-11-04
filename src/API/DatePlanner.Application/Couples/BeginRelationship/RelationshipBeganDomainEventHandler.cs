using DatePlanner.Application.Common.Events;
using DatePlanner.Application.Common.Exceptions;
using DatePlanner.Application.TurnGames.ScheduleTurnGame;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Couples.Events;
using MediatR;

namespace DatePlanner.Application.Couples.BeginRelationship;

public sealed class RelationshipBeganDomainEventHandler(
    ICoupleRepository coupleRepository,
    TurnGameScheduler turnGameScheduler)
    : INotificationHandler<DomainEventNotification<RelationshipBeganDomainEvent>>
{
    public async Task Handle(
        DomainEventNotification<RelationshipBeganDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        RelationshipBeganDomainEvent domainEvent = notification.DomainEvent;
        Couple? couple = await coupleRepository.GetCoupleByIdAsync(domainEvent.CoupleId);

        if (couple is null)
        {
            throw new DatePlannerException(
                nameof(RelationshipBeganDomainEvent),
                CoupleErrors.NotFound(domainEvent.PartnerOneId));
        }

        Result<Guid> result = await turnGameScheduler.ScheduleAsync(couple);

        if (result.IsFailure)
        {
            throw new DatePlannerException(nameof(RelationshipBeganDomainEvent), result.Error);
        }
    }
}

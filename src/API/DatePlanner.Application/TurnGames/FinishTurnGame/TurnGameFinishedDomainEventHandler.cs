using DatePlanner.Application.Common.Events;
using DatePlanner.Application.Common.Exceptions;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Dates;
using DatePlanner.Domain.TurnGames.Events;
using MediatR;

namespace DatePlanner.Application.TurnGames.FinishTurnGame;

public sealed class TurnGameFinishedDomainEventHandler(IDatePlanRepository datePlanRepository)
    : INotificationHandler<DomainEventNotification<TurnGameFinishedDomainEvent>>
{
    public Task Handle(
        DomainEventNotification<TurnGameFinishedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        TurnGameFinishedDomainEvent domainEvent = notification.DomainEvent;

        if (domainEvent.Purpose != Domain.TurnGames.TurnGamePurpose.PlanningDecision)
        {
            return Task.CompletedTask;
        }

        Result<DatePlan> result = DatePlan.Create(domainEvent.CoupleId, domainEvent.WinnerId);

        if (result.IsFailure)
        {
            throw new DatePlannerException(nameof(TurnGameFinishedDomainEvent), result.Error);
        }

        datePlanRepository.Insert(result.Value);

        return Task.CompletedTask;
    }
}

using DatePlanner.Application.Common.Events;
using DatePlanner.Application.Common.Exceptions;
using DatePlanner.Application.TurnGames.ScheduleTurnGame;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Dates.Events;
using MediatR;

namespace DatePlanner.Application.Dates.AttendDatePlan;

public sealed class DatePlanAttendedDomainEventHandler(
    ICoupleRepository coupleRepository,
    TurnGameScheduler turnGameScheduler)
    : INotificationHandler<DomainEventNotification<DatePlanAttendedDomainEvent>>
{
    public async Task Handle(
        DomainEventNotification<DatePlanAttendedDomainEvent> notification,
        CancellationToken cancellationToken)
    {
        DatePlanAttendedDomainEvent domainEvent = notification.DomainEvent;
        Couple? couple = await coupleRepository.GetCoupleByIdAsync(domainEvent.CoupleId);

        if (couple is null || !couple.IsActive)
        {
            return;
        }

        Result<Guid> result = await turnGameScheduler.ScheduleAsync(couple, cancellationToken);

        if (result.IsFailure)
        {
            throw new DatePlannerException(nameof(DatePlanAttendedDomainEvent), result.Error);
        }
    }
}

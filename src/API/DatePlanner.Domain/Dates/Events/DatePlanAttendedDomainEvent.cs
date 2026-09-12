using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates.Events;

public sealed class DatePlanAttendedDomainEvent : DomainEvent
{
    internal DatePlanAttendedDomainEvent(Guid datePlanId, Guid coupleId)
    {
        DatePlanId = datePlanId;
        CoupleId = coupleId;
    }

    public Guid DatePlanId { get; }

    public Guid CoupleId { get; }
}

using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Dates;
using MediatR;

namespace DatePlanner.Application.Dates.ListDatePlans;

public sealed record ListDatePlansQuery(Guid UserId)
    : IRequest<Result<IReadOnlyCollection<DatePlanSummaryResponse>>>;

public sealed record DatePlanSummaryResponse(
    Guid Id,
    Guid PartnerWhoPlannedId,
    string PlannerName,
    bool IsCollaborative,
    DateTime CreatedAtUtc,
    DateOnly? ScheduledFor,
    DateStatus Status,
    string Title,
    int ItineraryItemCount);

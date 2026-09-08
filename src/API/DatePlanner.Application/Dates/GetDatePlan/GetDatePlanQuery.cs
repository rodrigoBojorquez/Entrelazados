using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Dates;
using MediatR;

namespace DatePlanner.Application.Dates.GetDatePlan;

public sealed record GetDatePlanQuery(Guid UserId, Guid DatePlanId) : IRequest<Result<DatePlanResponse>>;

public sealed record DatePlanResponse(
    Guid Id,
    Guid CoupleId,
    Guid PartnerWhoPlannedId,
    DateOnly? ScheduledFor,
    DateStatus Status,
    IReadOnlyCollection<ItineraryItemResponse> Itinerary);

public sealed record ItineraryItemResponse(
    Guid Id,
    int Order,
    string Name,
    TimeOnly? StartsAt,
    decimal? Latitude,
    decimal? Longitude);

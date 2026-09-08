using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Dates.RemoveItineraryItem;

public sealed record RemoveItineraryItemCommand(
    Guid UserId,
    Guid DatePlanId,
    Guid ItineraryItemId) : IRequest<Result>;
using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Dates.AddItineraryItem;

public sealed record AddItineraryItemCommand(
    Guid UserId,
    Guid DatePlanId,
    string Name,
    decimal? Latitude,
    decimal? Longitude,
    TimeOnly? StartsAt) : IRequest<Result>;
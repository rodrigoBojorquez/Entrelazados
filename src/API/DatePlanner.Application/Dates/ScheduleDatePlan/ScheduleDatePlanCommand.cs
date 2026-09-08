using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Dates.ScheduleDatePlan;

public sealed record ScheduleDatePlanCommand(
    Guid UserId,
    Guid DatePlanId,
    DateOnly ScheduledFor) : IRequest<Result>;
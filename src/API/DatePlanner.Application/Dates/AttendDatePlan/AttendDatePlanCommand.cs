using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Dates.AttendDatePlan;

public sealed record AttendDatePlanCommand(Guid UserId, Guid DatePlanId) : IRequest<Result>;
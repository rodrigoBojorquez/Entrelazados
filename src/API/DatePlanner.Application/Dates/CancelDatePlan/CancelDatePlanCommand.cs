using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Dates.CancelDatePlan;

public sealed record CancelDatePlanCommand(Guid UserId, Guid DatePlanId) : IRequest<Result>;
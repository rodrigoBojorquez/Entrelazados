using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Dates.CreateDatePlan;

public sealed record CreateDatePlanCommand(Guid UserId) : IRequest<Result<Guid>>;
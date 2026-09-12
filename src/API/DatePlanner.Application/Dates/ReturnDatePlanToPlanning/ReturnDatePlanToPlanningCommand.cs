using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Dates.ReturnDatePlanToPlanning;

public sealed record ReturnDatePlanToPlanningCommand(
    Guid UserId,
    Guid DatePlanId) : IRequest<Result>;

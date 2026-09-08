using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.Dates.ScheduleDatePlan;

public sealed record ScheduleDatePlanCommand(
    Guid UserId,
    Guid DatePlanId,
    DateOnly ScheduledFor) : IRequest<Result>;

internal sealed class ScheduleDatePlanCommandValidator : AbstractValidator<ScheduleDatePlanCommand>
{
    public ScheduleDatePlanCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DatePlanId).NotEmpty();
        RuleFor(command => command.ScheduledFor).NotEmpty();
    }
}

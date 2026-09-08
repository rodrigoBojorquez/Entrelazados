using FluentValidation;

namespace DatePlanner.Application.Dates.ScheduleDatePlan;

internal sealed class ScheduleDatePlanCommandValidator : AbstractValidator<ScheduleDatePlanCommand>
{
    public ScheduleDatePlanCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DatePlanId).NotEmpty();
        RuleFor(command => command.ScheduledFor).NotEmpty();
    }
}
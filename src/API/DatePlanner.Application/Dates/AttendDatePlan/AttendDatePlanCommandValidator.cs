using FluentValidation;

namespace DatePlanner.Application.Dates.AttendDatePlan;

internal sealed class AttendDatePlanCommandValidator : AbstractValidator<AttendDatePlanCommand>
{
    public AttendDatePlanCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DatePlanId).NotEmpty();
    }
}
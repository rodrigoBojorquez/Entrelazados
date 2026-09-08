using FluentValidation;

namespace DatePlanner.Application.Dates.CancelDatePlan;

internal sealed class CancelDatePlanCommandValidator : AbstractValidator<CancelDatePlanCommand>
{
    public CancelDatePlanCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DatePlanId).NotEmpty();
    }
}
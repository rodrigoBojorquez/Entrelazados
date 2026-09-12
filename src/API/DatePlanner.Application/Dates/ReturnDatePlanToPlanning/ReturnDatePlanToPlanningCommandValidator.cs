using FluentValidation;

namespace DatePlanner.Application.Dates.ReturnDatePlanToPlanning;

internal sealed class ReturnDatePlanToPlanningCommandValidator
    : AbstractValidator<ReturnDatePlanToPlanningCommand>
{
    public ReturnDatePlanToPlanningCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.DatePlanId).NotEmpty();
    }
}

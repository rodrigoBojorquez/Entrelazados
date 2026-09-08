using FluentValidation;

namespace DatePlanner.Application.Dates.CreateDatePlan;

internal sealed class CreateDatePlanCommandValidator : AbstractValidator<CreateDatePlanCommand>
{
    public CreateDatePlanCommandValidator() => RuleFor(command => command.UserId).NotEmpty();
}
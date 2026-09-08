using FluentValidation;

namespace DatePlanner.Application.Dates.GetDatePlan;

internal sealed class GetDatePlanQueryValidator : AbstractValidator<GetDatePlanQuery>
{
    public GetDatePlanQueryValidator()
    {
        RuleFor(query => query.UserId).NotEmpty();
        RuleFor(query => query.DatePlanId).NotEmpty();
    }
}
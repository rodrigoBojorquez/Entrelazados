using FluentValidation;

namespace DatePlanner.Application.Dates.ListDatePlans;

internal sealed class ListDatePlansQueryValidator : AbstractValidator<ListDatePlansQuery>
{
    public ListDatePlansQueryValidator() => RuleFor(query => query.UserId).NotEmpty();
}

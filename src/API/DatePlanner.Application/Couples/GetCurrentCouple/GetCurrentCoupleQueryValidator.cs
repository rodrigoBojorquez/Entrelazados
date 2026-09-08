using FluentValidation;

namespace DatePlanner.Application.Couples.GetCurrentCouple;

internal sealed class GetCurrentCoupleQueryValidator : AbstractValidator<GetCurrentCoupleQuery>
{
    public GetCurrentCoupleQueryValidator() => RuleFor(query => query.UserId).NotEmpty();
}
using FluentValidation;

namespace DatePlanner.Application.Couples.GetCurrentPartner;

public sealed class GetCurrentPartnerQueryValidator : AbstractValidator<GetCurrentPartnerQuery>
{
    public GetCurrentPartnerQueryValidator()
    {
        RuleFor(query => query.UserId).NotEmpty();
    }
}
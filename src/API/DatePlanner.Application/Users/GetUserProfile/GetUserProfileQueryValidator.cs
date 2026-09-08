using FluentValidation;

namespace DatePlanner.Application.Users.GetUserProfile;

internal sealed class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator() => RuleFor(query => query.UserId).NotEmpty();
}
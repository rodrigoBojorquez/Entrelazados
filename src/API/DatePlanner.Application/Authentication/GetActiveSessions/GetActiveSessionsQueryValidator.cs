using FluentValidation;

namespace DatePlanner.Application.Authentication.GetActiveSessions;

internal sealed class GetActiveSessionsQueryValidator : AbstractValidator<GetActiveSessionsQuery>
{
    public GetActiveSessionsQueryValidator() =>
        RuleFor(query => query.UserId).NotEmpty();
}

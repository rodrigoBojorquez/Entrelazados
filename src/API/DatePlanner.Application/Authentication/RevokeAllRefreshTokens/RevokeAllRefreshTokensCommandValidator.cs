using FluentValidation;

namespace DatePlanner.Application.Authentication.RevokeAllRefreshTokens;

internal sealed class RevokeAllRefreshTokensCommandValidator
    : AbstractValidator<RevokeAllRefreshTokensCommand>
{
    public RevokeAllRefreshTokensCommandValidator() =>
        RuleFor(command => command.UserId).NotEmpty();
}

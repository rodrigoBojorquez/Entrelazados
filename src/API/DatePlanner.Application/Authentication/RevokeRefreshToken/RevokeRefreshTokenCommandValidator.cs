using FluentValidation;

namespace DatePlanner.Application.Authentication.RevokeRefreshToken;

internal sealed class RevokeRefreshTokenCommandValidator : AbstractValidator<RevokeRefreshTokenCommand>
{
    public RevokeRefreshTokenCommandValidator()
    {
        RuleFor(command => command.RefreshToken)
            .NotEmpty()
            .MaximumLength(256);
    }
}

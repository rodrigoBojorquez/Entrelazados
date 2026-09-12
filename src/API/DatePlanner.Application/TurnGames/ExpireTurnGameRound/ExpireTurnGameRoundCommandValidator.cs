using FluentValidation;

namespace DatePlanner.Application.TurnGames.ExpireTurnGameRound;

internal sealed class ExpireTurnGameRoundCommandValidator
    : AbstractValidator<ExpireTurnGameRoundCommand>
{
    public ExpireTurnGameRoundCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.TurnGameId).NotEmpty();
        RuleFor(command => command.RoundId).NotEmpty();
    }
}

using FluentValidation;

namespace DatePlanner.Application.TurnGames.StartTurnGameRound;

internal sealed class StartTurnGameRoundCommandValidator : AbstractValidator<StartTurnGameRoundCommand>
{
    public StartTurnGameRoundCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.TurnGameId).NotEmpty();
    }
}
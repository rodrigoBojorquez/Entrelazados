using FluentValidation;

namespace DatePlanner.Application.TurnGames.PlayTurnGameMove;

internal sealed class PlayTurnGameMoveCommandValidator : AbstractValidator<PlayTurnGameMoveCommand>
{
    public PlayTurnGameMoveCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.TurnGameId).NotEmpty();
        RuleFor(command => command.MoveCode).NotEmpty().MaximumLength(32);
    }
}
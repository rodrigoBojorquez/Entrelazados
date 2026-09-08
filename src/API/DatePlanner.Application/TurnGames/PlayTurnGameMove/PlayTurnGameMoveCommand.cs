using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.TurnGames.PlayTurnGameMove;

public sealed record PlayTurnGameMoveCommand(
    Guid UserId,
    Guid TurnGameId,
    string MoveCode) : IRequest<Result>;

internal sealed class PlayTurnGameMoveCommandValidator : AbstractValidator<PlayTurnGameMoveCommand>
{
    public PlayTurnGameMoveCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.TurnGameId).NotEmpty();
        RuleFor(command => command.MoveCode).NotEmpty().MaximumLength(32);
    }
}

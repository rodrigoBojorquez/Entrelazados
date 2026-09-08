using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.TurnGames.PlayTurnGameMove;

public sealed record PlayTurnGameMoveCommand(
    Guid UserId,
    Guid TurnGameId,
    string MoveCode) : IRequest<Result>;
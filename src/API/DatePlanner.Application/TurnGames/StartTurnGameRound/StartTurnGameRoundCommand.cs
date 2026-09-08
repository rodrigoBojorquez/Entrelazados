using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.TurnGames.StartTurnGameRound;

public sealed record StartTurnGameRoundCommand(
    Guid UserId,
    Guid TurnGameId) : IRequest<Result>;
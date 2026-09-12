using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.TurnGames.ExpireTurnGameRound;

public sealed record ExpireTurnGameRoundCommand(
    Guid UserId,
    Guid TurnGameId,
    Guid RoundId) : IRequest<Result>;

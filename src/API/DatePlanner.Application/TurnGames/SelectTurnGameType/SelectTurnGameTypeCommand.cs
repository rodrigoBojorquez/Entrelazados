using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using MediatR;

namespace DatePlanner.Application.TurnGames.SelectTurnGameType;

public sealed record SelectTurnGameTypeCommand(
    Guid UserId,
    Guid TurnGameId,
    TurnGameType GameType) : IRequest<Result>;

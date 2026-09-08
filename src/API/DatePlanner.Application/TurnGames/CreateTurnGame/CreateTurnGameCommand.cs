using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using MediatR;

namespace DatePlanner.Application.TurnGames.CreateTurnGame;

public sealed record CreateTurnGameCommand(
    Guid UserId,
    TurnGameType GameType) : IRequest<Result<Guid>>;
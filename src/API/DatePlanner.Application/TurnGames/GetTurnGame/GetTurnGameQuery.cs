using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.TurnGames.GetTurnGame;

public sealed record GetTurnGameQuery(Guid UserId, Guid TurnGameId)
    : IRequest<Result<TurnGameResponse>>;
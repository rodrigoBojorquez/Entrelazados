using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using MediatR;

namespace DatePlanner.Application.TurnGames.GetCurrentScheduledTurnGame;

public sealed record GetCurrentScheduledTurnGameQuery(Guid UserId)
    : IRequest<Result<CurrentScheduledTurnGameResponse>>;

public sealed record CurrentScheduledTurnGameResponse(ScheduledTurnGameResponse? Game);

public sealed record ScheduledTurnGameResponse(
    Guid Id,
    TurnGameType? GameType,
    TurnGameStatus Status,
    DateTime CreatedAtUtc);

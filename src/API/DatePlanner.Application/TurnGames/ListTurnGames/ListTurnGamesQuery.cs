using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using MediatR;

namespace DatePlanner.Application.TurnGames.ListTurnGames;

public sealed record ListTurnGamesQuery(Guid UserId)
    : IRequest<Result<IReadOnlyCollection<TurnGameSummaryResponse>>>;

public sealed record TurnGameSummaryResponse(
    Guid Id,
    TurnGameType? GameType,
    TurnGamePurpose Purpose,
    TurnGameStatus Status,
    Guid? WinnerId,
    string? WinnerName,
    DateTime CreatedAtUtc,
    DateTime? FinishedAtUtc,
    int RoundsPlayed);

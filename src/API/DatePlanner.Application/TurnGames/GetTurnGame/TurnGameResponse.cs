using DatePlanner.Domain.TurnGames;

namespace DatePlanner.Application.TurnGames.GetTurnGame;

public sealed record TurnGameResponse(
    Guid Id,
    Guid CoupleId,
    Guid PlayerOneId,
    Guid PlayerTwoId,
    TurnGameType GameType,
    TurnGameStatus Status,
    Guid? WinnerId,
    DateTime CreatedAtUtc,
    DateTime? StartedAtUtc,
    DateTime? FinishedAtUtc,
    IReadOnlyCollection<TurnGameRoundResponse> Rounds);
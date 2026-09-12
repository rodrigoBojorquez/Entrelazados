using DatePlanner.Domain.TurnGames;

namespace DatePlanner.Application.TurnGames.GetTurnGame;

public sealed record TurnGameResponse(
    Guid Id,
    Guid CoupleId,
    Guid PlayerOneId,
    Guid PlayerTwoId,
    TurnGameType? GameType,
    TurnGamePurpose Purpose,
    TurnGameStatus Status,
    Guid? WinnerId,
    DateTime CreatedAtUtc,
    DateTime? StartedAtUtc,
    DateTime? FinishedAtUtc,
    DateTime ServerTimeUtc,
    IReadOnlyCollection<TurnGameRoundResponse> Rounds);

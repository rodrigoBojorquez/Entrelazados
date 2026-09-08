using DatePlanner.Domain.TurnGames.Rounds;

namespace DatePlanner.Application.TurnGames.GetTurnGame;

public sealed record TurnGameRoundResponse(
    Guid Id,
    int Number,
    TurnGameRoundStatus Status,
    Guid? WinnerId,
    DateTime StartedAtUtc,
    DateTime? FinishedAtUtc,
    IReadOnlyCollection<TurnGameMoveResponse> Moves);
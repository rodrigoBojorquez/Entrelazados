namespace DatePlanner.Application.TurnGames.GetTurnGame;

public sealed record TurnGameMoveResponse(
    Guid Id,
    Guid PlayerId,
    int Sequence,
    string? Code,
    DateTime PlayedAtUtc);
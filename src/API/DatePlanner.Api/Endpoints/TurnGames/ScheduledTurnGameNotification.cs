namespace DatePlanner.Api.Endpoints.TurnGames;

public sealed record ScheduledTurnGameNotification(Guid TurnGameId);

public sealed record TurnGamePresenceResponse(
    Guid TurnGameId,
    IReadOnlyCollection<Guid> ConnectedPlayerIds,
    bool BothPlayersConnected);

public sealed record TurnGamePresenceJoinResult(
    TurnGamePresenceResponse Presence,
    bool BecameFullyConnected);

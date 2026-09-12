namespace DatePlanner.Api.Endpoints.TurnGames;

public sealed class TurnGamePresenceTracker
{
    private readonly Lock _lock = new();
    private readonly Dictionary<Guid, Dictionary<Guid, HashSet<string>>> _connections = [];
    private readonly Dictionary<string, HashSet<Guid>> _gamesByConnection = [];

    public TurnGamePresenceJoinResult Join(
        Guid turnGameId,
        Guid userId,
        string connectionId,
        Guid playerOneId,
        Guid playerTwoId)
    {
        lock (_lock)
        {
            if (!_connections.TryGetValue(turnGameId, out Dictionary<Guid, HashSet<string>>? players))
            {
                players = [];
                _connections[turnGameId] = players;
            }

            if (!players.TryGetValue(userId, out HashSet<string>? connections))
            {
                connections = [];
                players[userId] = connections;
            }

            bool bothPlayersWereConnected =
                IsConnected(players, playerOneId) && IsConnected(players, playerTwoId);

            connections.Add(connectionId);

            if (!_gamesByConnection.TryGetValue(connectionId, out HashSet<Guid>? games))
            {
                games = [];
                _gamesByConnection[connectionId] = games;
            }

            games.Add(turnGameId);
            TurnGamePresenceResponse presence =
                Snapshot(turnGameId, players, playerOneId, playerTwoId);

            return new TurnGamePresenceJoinResult(
                presence,
                !bothPlayersWereConnected && presence.BothPlayersConnected);
        }
    }

    public bool AreBothPlayersConnected(Guid turnGameId, Guid playerOneId, Guid playerTwoId)
    {
        lock (_lock)
        {
            return _connections.TryGetValue(turnGameId, out Dictionary<Guid, HashSet<string>>? players)
                && IsConnected(players, playerOneId)
                && IsConnected(players, playerTwoId);
        }
    }

    public IReadOnlyCollection<Guid> Leave(string connectionId)
    {
        lock (_lock)
        {
            if (!_gamesByConnection.Remove(connectionId, out HashSet<Guid>? gameIds))
            {
                return [];
            }

            foreach (Guid gameId in gameIds)
            {
                if (!_connections.TryGetValue(gameId, out Dictionary<Guid, HashSet<string>>? players))
                {
                    continue;
                }

                foreach ((Guid userId, HashSet<string> connections) in players.ToArray())
                {
                    connections.Remove(connectionId);
                    if (connections.Count == 0) players.Remove(userId);
                }

                if (players.Count == 0) _connections.Remove(gameId);
            }

            return gameIds.ToArray();
        }
    }

    public TurnGamePresenceResponse GetSnapshot(
        Guid turnGameId,
        Guid playerOneId,
        Guid playerTwoId)
    {
        lock (_lock)
        {
            _connections.TryGetValue(turnGameId, out Dictionary<Guid, HashSet<string>>? players);
            return Snapshot(turnGameId, players ?? [], playerOneId, playerTwoId);
        }
    }

    private static TurnGamePresenceResponse Snapshot(
        Guid turnGameId,
        Dictionary<Guid, HashSet<string>> players,
        Guid playerOneId,
        Guid playerTwoId)
    {
        Guid[] connected = players
            .Where(entry => entry.Value.Count > 0)
            .Select(entry => entry.Key)
            .ToArray();

        return new TurnGamePresenceResponse(
            turnGameId,
            connected,
            IsConnected(players, playerOneId) && IsConnected(players, playerTwoId));
    }

    private static bool IsConnected(
        Dictionary<Guid, HashSet<string>> players,
        Guid playerId) =>
        players.TryGetValue(playerId, out HashSet<string>? connections) && connections.Count > 0;
}

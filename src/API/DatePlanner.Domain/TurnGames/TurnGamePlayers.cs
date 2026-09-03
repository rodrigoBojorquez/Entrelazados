using DatePlanner.Domain.TurnGames.Rules;

namespace DatePlanner.Domain.TurnGames;

internal readonly record struct TurnGamePlayers(Guid PlayerOneId, Guid PlayerTwoId)
{
    internal bool TryGetPosition(Guid playerId, out PlayerPosition position)
    {
        if (playerId == PlayerOneId)
        {
            position = PlayerPosition.PlayerOne;
            return true;
        }

        if (playerId == PlayerTwoId)
        {
            position = PlayerPosition.PlayerTwo;
            return true;
        }

        position = default;
        return false;
    }

    internal Guid GetId(PlayerPosition position) =>
        position switch
        {
            PlayerPosition.PlayerOne => PlayerOneId,
            PlayerPosition.PlayerTwo => PlayerTwoId,
            _ => throw new ArgumentOutOfRangeException(
                nameof(position))
        };
}

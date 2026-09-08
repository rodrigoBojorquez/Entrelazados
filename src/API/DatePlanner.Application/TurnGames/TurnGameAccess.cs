using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;

namespace DatePlanner.Application.TurnGames;

internal static class TurnGameAccess
{
    internal static async Task<Result<TurnGame>> GetForPlayerAsync(
        Guid turnGameId,
        Guid userId,
        ITurnGameRepository turnGameRepository)
    {
        TurnGame? turnGame = await turnGameRepository.GetTurnGameByIdAsync(turnGameId);
        bool isPlayer = turnGame is not null &&
                        (turnGame.PlayerOneId == userId || turnGame.PlayerTwoId == userId);

        return isPlayer
            ? turnGame!
            : Result.Failure<TurnGame>(TurnGameErrors.NotFound(turnGameId));
    }
}

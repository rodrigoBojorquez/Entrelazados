namespace DatePlanner.Domain.TurnGames;

public interface ITurnGameRepository
{
    Task<TurnGame?> GetTurnGameByIdAsync(Guid turnGameId);

    Task<TurnGame?> GetActivePlanningGameByCoupleIdAsync(Guid coupleId);

    void Insert(TurnGame turnGame);
}

namespace DatePlanner.Domain.TurnGames;

public interface ITurnGameRepository
{
    Task<TurnGame?> GetTurnGameByIdAsync(Guid turnGameId);

    void Insert(TurnGame turnGame);
}

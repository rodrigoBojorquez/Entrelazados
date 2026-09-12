using DatePlanner.Domain.TurnGames;
using DatePlanner.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace DatePlanner.Infrastructure.TurnGames;

internal sealed class TurnGameRepository(DatePlannerDbContext dbContext) : ITurnGameRepository
{
    public Task<TurnGame?> GetTurnGameByIdAsync(Guid turnGameId) =>
        dbContext.TurnGames
            .Include(turnGame => turnGame.Rounds)
            .ThenInclude(round => round.Moves)
            .AsSplitQuery()
            .FirstOrDefaultAsync(turnGame => turnGame.Id == turnGameId);

    public async Task<TurnGame?> GetActivePlanningGameByCoupleIdAsync(Guid coupleId)
    {
        TurnGame? localGame = dbContext.TurnGames.Local.FirstOrDefault(turnGame =>
            turnGame.CoupleId == coupleId &&
            turnGame.Purpose == TurnGamePurpose.PlanningDecision &&
            turnGame.Status != TurnGameStatus.Finished);

        return localGame ?? await dbContext.TurnGames.FirstOrDefaultAsync(turnGame =>
            turnGame.CoupleId == coupleId &&
            turnGame.Purpose == TurnGamePurpose.PlanningDecision &&
            turnGame.Status != TurnGameStatus.Finished);
    }

    public void Insert(TurnGame turnGame) => dbContext.TurnGames.Add(turnGame);
}

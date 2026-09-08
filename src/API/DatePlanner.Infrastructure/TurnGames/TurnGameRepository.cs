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

    public void Insert(TurnGame turnGame) => dbContext.TurnGames.Add(turnGame);
}

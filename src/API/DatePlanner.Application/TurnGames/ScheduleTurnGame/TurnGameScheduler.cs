using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.TurnGames;

namespace DatePlanner.Application.TurnGames.ScheduleTurnGame;

public sealed class TurnGameScheduler(
    ITurnGameRepository turnGameRepository)
{
    public async Task<Result<Guid>> ScheduleAsync(Couple couple)
    {
        TurnGame? existingGame =
            await turnGameRepository.GetActivePlanningGameByCoupleIdAsync(couple.Id);

        if (existingGame is not null)
        {
            return existingGame.Id;
        }

        Result<TurnGame> creationResult = TurnGame.Schedule(couple);

        if (creationResult.IsFailure)
        {
            return Result.Failure<Guid>(creationResult.Error);
        }

        TurnGame turnGame = creationResult.Value;
        turnGameRepository.Insert(turnGame);

        return turnGame.Id;
    }
}

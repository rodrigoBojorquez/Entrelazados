using DatePlanner.Application.Common.Notifications;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.TurnGames;

namespace DatePlanner.Application.TurnGames.ScheduleTurnGame;

public sealed class TurnGameScheduler(
    ITurnGameRepository turnGameRepository,
    ITurnGameNotifier notifier)
{
    public async Task<Result<Guid>> ScheduleAsync(
        Couple couple,
        CancellationToken cancellationToken)
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

        await notifier.NotifyGameScheduledAsync(
            turnGame.Id,
            turnGame.PlayerOneId,
            turnGame.PlayerTwoId,
            cancellationToken);

        return turnGame.Id;
    }
}

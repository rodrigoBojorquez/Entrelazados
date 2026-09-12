using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using MediatR;

namespace DatePlanner.Application.TurnGames.ExpireTurnGameRound;

public sealed class ExpireTurnGameRoundCommandHandler(
    ITurnGameRepository turnGameRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<ExpireTurnGameRoundCommand, Result>
{
    public async Task<Result> Handle(
        ExpireTurnGameRoundCommand request,
        CancellationToken cancellationToken)
    {
        Result<TurnGame> accessResult = await TurnGameAccess.GetForPlayerAsync(
            request.TurnGameId,
            request.UserId,
            turnGameRepository);

        if (accessResult.IsFailure)
        {
            return Result.Failure(accessResult.Error);
        }

        Result result = accessResult.Value.ExpireRound(
            request.RoundId,
            timeProvider.GetUtcNow().UtcDateTime);

        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

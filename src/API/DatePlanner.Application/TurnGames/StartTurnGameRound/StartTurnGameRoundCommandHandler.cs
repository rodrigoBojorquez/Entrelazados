using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using MediatR;

namespace DatePlanner.Application.TurnGames.StartTurnGameRound;

public sealed class StartTurnGameRoundCommandHandler(
    ITurnGameRepository turnGameRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<StartTurnGameRoundCommand, Result>
{
    public async Task<Result> Handle(
        StartTurnGameRoundCommand request,
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

        Result result = accessResult.Value.StartRound();

        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

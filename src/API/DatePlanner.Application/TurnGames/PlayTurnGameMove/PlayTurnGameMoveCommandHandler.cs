using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.TurnGames.Rules;
using MediatR;

namespace DatePlanner.Application.TurnGames.PlayTurnGameMove;

public sealed class PlayTurnGameMoveCommandHandler(
    ITurnGameRepository turnGameRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<PlayTurnGameMoveCommand, Result>
{
    public async Task<Result> Handle(
        PlayTurnGameMoveCommand request,
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

        TurnGame turnGame = accessResult.Value;
        Result<ITurnGameMove> moveResult = TurnGameMoveFactory.Create(
            turnGame.GameType,
            request.MoveCode);

        if (moveResult.IsFailure)
        {
            return Result.Failure(moveResult.Error);
        }

        Result result = turnGame.Play(request.UserId, moveResult.Value);

        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

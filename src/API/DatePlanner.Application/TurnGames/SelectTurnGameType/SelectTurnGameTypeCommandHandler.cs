using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using MediatR;

namespace DatePlanner.Application.TurnGames.SelectTurnGameType;

public sealed class SelectTurnGameTypeCommandHandler(
    ITurnGameRepository turnGameRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<SelectTurnGameTypeCommand, Result>
{
    public async Task<Result> Handle(
        SelectTurnGameTypeCommand request,
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

        Result selectionResult = accessResult.Value.SelectGameType(request.GameType);

        if (selectionResult.IsFailure)
        {
            return selectionResult;
        }

        bool saved = await unitOfWork.TrySaveChangesAsync(cancellationToken);
        return saved
            ? Result.Success()
            : Result.Failure(TurnGameErrors.GameTypeAlreadySelected);
    }
}

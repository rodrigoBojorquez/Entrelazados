using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.TurnGames;
using MediatR;

namespace DatePlanner.Application.TurnGames.CreateTurnGame;

public sealed class CreateTurnGameCommandHandler(
    ICoupleRepository coupleRepository,
    ITurnGameRepository turnGameRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateTurnGameCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateTurnGameCommand request,
        CancellationToken cancellationToken)
    {
        Couple? couple = await coupleRepository.GetCoupleByUserIdAsync(request.UserId);

        if (couple is null || !couple.IsActive)
        {
            return Result.Failure<Guid>(CoupleErrors.NotFound(request.UserId));
        }

        Result<TurnGame> creationResult = TurnGame.Create(couple, request.GameType);

        if (creationResult.IsFailure)
        {
            return Result.Failure<Guid>(creationResult.Error);
        }

        turnGameRepository.Insert(creationResult.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return creationResult.Value.Id;
    }
}

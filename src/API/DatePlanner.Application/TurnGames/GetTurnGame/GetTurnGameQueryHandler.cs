using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.TurnGames.Rounds;
using MediatR;

namespace DatePlanner.Application.TurnGames.GetTurnGame;

public sealed class GetTurnGameQueryHandler(ITurnGameRepository turnGameRepository)
    : IRequestHandler<GetTurnGameQuery, Result<TurnGameResponse>>
{
    public async Task<Result<TurnGameResponse>> Handle(
        GetTurnGameQuery request,
        CancellationToken cancellationToken)
    {
        Result<TurnGame> accessResult = await TurnGameAccess.GetForPlayerAsync(
            request.TurnGameId,
            request.UserId,
            turnGameRepository);

        if (accessResult.IsFailure)
        {
            return Result.Failure<TurnGameResponse>(accessResult.Error);
        }

        TurnGame turnGame = accessResult.Value;

        return new TurnGameResponse(
            turnGame.Id,
            turnGame.CoupleId,
            turnGame.PlayerOneId,
            turnGame.PlayerTwoId,
            turnGame.GameType,
            turnGame.Status,
            turnGame.WinnerId,
            turnGame.CreatedAtUtc,
            turnGame.StartedAtUtc,
            turnGame.FinishedAtUtc,
            turnGame.Rounds
                .OrderBy(round => round.Number)
                .Select(round => MapRound(round, request.UserId))
                .ToArray());
    }

    private static TurnGameRoundResponse MapRound(TurnGameRound round, Guid requestingUserId)
    {
        bool revealAllMoves = round.Status == TurnGameRoundStatus.Finished;

        return new TurnGameRoundResponse(
            round.Id,
            round.Number,
            round.Status,
            round.WinnerId,
            round.StartedAtUtc,
            round.FinishedAtUtc,
            round.Moves
                .OrderBy(move => move.Sequence)
                .Select(move => new TurnGameMoveResponse(
                    move.Id,
                    move.PlayerId,
                    move.Sequence,
                    revealAllMoves || move.PlayerId == requestingUserId ? move.Code : null,
                    move.PlayedAtUtc))
                .ToArray());
    }
}

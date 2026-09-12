using System.Data.Common;
using Dapper;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.TurnGames.Rounds;
using MediatR;

namespace DatePlanner.Application.TurnGames.GetTurnGame;

public sealed class GetTurnGameQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    TimeProvider timeProvider)
    : IRequestHandler<GetTurnGameQuery, Result<TurnGameResponse>>
{
    public async Task<Result<TurnGameResponse>> Handle(
        GetTurnGameQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 tg.id AS {nameof(TurnGameRow.Id)},
                 tg.couple_id AS {nameof(TurnGameRow.CoupleId)},
                 tg.player_one_id AS {nameof(TurnGameRow.PlayerOneId)},
                 tg.player_two_id AS {nameof(TurnGameRow.PlayerTwoId)},
                 tg.game_type AS {nameof(TurnGameRow.GameType)},
                 tg.purpose AS {nameof(TurnGameRow.Purpose)},
                 tg.status AS {nameof(TurnGameRow.Status)},
                 tg.winner_id AS {nameof(TurnGameRow.WinnerId)},
                 tg.created_at_utc AS {nameof(TurnGameRow.CreatedAtUtc)},
                 tg.started_at_utc AS {nameof(TurnGameRow.StartedAtUtc)},
                 tg.finished_at_utc AS {nameof(TurnGameRow.FinishedAtUtc)}
             FROM dateplanner.turn_games tg
             WHERE tg.id = @TurnGameId
               AND (tg.player_one_id = @UserId OR tg.player_two_id = @UserId);

             SELECT
                 r.id AS {nameof(TurnGameRoundRow.Id)},
                 r.number AS {nameof(TurnGameRoundRow.Number)},
                 r.status AS {nameof(TurnGameRoundRow.Status)},
                 r.winner_id AS {nameof(TurnGameRoundRow.WinnerId)},
                 r.started_at_utc AS {nameof(TurnGameRoundRow.StartedAtUtc)},
                 r.finished_at_utc AS {nameof(TurnGameRoundRow.FinishedAtUtc)}
             FROM dateplanner.turn_game_rounds r
             INNER JOIN dateplanner.turn_games tg ON tg.id = r.turn_game_id
             WHERE r.turn_game_id = @TurnGameId
               AND (tg.player_one_id = @UserId OR tg.player_two_id = @UserId)
             ORDER BY r.number;

             SELECT
                 m.id AS {nameof(TurnGameMoveRow.Id)},
                 m.turn_game_round_id AS {nameof(TurnGameMoveRow.TurnGameRoundId)},
                 m.player_id AS {nameof(TurnGameMoveRow.PlayerId)},
                 m.sequence AS {nameof(TurnGameMoveRow.Sequence)},
                 CASE
                     WHEN r.status = 'Finished' OR m.player_id = @UserId THEN m.code
                     ELSE NULL
                 END AS {nameof(TurnGameMoveRow.Code)},
                 m.played_at_utc AS {nameof(TurnGameMoveRow.PlayedAtUtc)}
             FROM dateplanner.turn_game_moves m
             INNER JOIN dateplanner.turn_game_rounds r ON r.id = m.turn_game_round_id
             INNER JOIN dateplanner.turn_games tg ON tg.id = r.turn_game_id
             WHERE r.turn_game_id = @TurnGameId
               AND (tg.player_one_id = @UserId OR tg.player_two_id = @UserId)
             ORDER BY r.number, m.sequence;
             """;

        var command = new CommandDefinition(sql, request, cancellationToken: cancellationToken);
        using SqlMapper.GridReader gridReader = await connection.QueryMultipleAsync(command);

        TurnGameRow? turnGame = await gridReader.ReadSingleOrDefaultAsync<TurnGameRow>();

        if (turnGame is null)
        {
            return Result.Failure<TurnGameResponse>(TurnGameErrors.NotFound(request.TurnGameId));
        }

        TurnGameRoundRow[] rounds = (await gridReader.ReadAsync<TurnGameRoundRow>()).ToArray();
        ILookup<Guid, TurnGameMoveRow> movesByRound =
            (await gridReader.ReadAsync<TurnGameMoveRow>()).ToLookup(move => move.TurnGameRoundId);

        return new TurnGameResponse(
            turnGame.Id,
            turnGame.CoupleId,
            turnGame.PlayerOneId,
            turnGame.PlayerTwoId,
            string.IsNullOrWhiteSpace(turnGame.GameType)
                ? null
                : Enum.Parse<TurnGameType>(turnGame.GameType),
            Enum.Parse<TurnGamePurpose>(turnGame.Purpose),
            Enum.Parse<TurnGameStatus>(turnGame.Status),
            turnGame.WinnerId,
            turnGame.CreatedAtUtc,
            turnGame.StartedAtUtc,
            turnGame.FinishedAtUtc,
            timeProvider.GetUtcNow().UtcDateTime,
            rounds
                .Select(round => MapRound(round, movesByRound[round.Id]))
                .ToArray());
    }

    private static TurnGameRoundResponse MapRound(
        TurnGameRoundRow round,
        IEnumerable<TurnGameMoveRow> moves)
    {
        return new TurnGameRoundResponse(
            round.Id,
            round.Number,
            Enum.Parse<TurnGameRoundStatus>(round.Status),
            round.WinnerId,
            round.StartedAtUtc,
            round.FinishedAtUtc,
            moves
                .Select(move => new TurnGameMoveResponse(
                    move.Id,
                    move.PlayerId,
                    move.Sequence,
                    move.Code,
                    move.PlayedAtUtc))
                .ToArray());
    }

    private sealed class TurnGameRow
    {
        public Guid Id { get; init; }
        public Guid CoupleId { get; init; }
        public Guid PlayerOneId { get; init; }
        public Guid PlayerTwoId { get; init; }
        public string? GameType { get; init; }
        public string Purpose { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public Guid? WinnerId { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateTime? StartedAtUtc { get; init; }
        public DateTime? FinishedAtUtc { get; init; }
    }

    private sealed class TurnGameRoundRow
    {
        public Guid Id { get; init; }
        public int Number { get; init; }
        public string Status { get; init; } = string.Empty;
        public Guid? WinnerId { get; init; }
        public DateTime StartedAtUtc { get; init; }
        public DateTime? FinishedAtUtc { get; init; }
    }

    private sealed class TurnGameMoveRow
    {
        public Guid Id { get; init; }
        public Guid TurnGameRoundId { get; init; }
        public Guid PlayerId { get; init; }
        public int Sequence { get; init; }
        public string? Code { get; init; }
        public DateTime PlayedAtUtc { get; init; }
    }
}

using System.Data.Common;
using Dapper;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using MediatR;

namespace DatePlanner.Application.TurnGames.ListTurnGames;

public sealed class ListTurnGamesQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IRequestHandler<ListTurnGamesQuery, Result<IReadOnlyCollection<TurnGameSummaryResponse>>>
{
    public async Task<Result<IReadOnlyCollection<TurnGameSummaryResponse>>> Handle(
        ListTurnGamesQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 tg.id AS {nameof(TurnGameRow.Id)},
                 tg.game_type AS {nameof(TurnGameRow.GameType)},
                 tg.purpose AS {nameof(TurnGameRow.Purpose)},
                 tg.status AS {nameof(TurnGameRow.Status)},
                 tg.winner_id AS {nameof(TurnGameRow.WinnerId)},
                 CASE WHEN winner.id IS NULL THEN NULL
                      ELSE winner.first_name || ' ' || winner.last_name
                 END AS {nameof(TurnGameRow.WinnerName)},
                 tg.created_at_utc AS {nameof(TurnGameRow.CreatedAtUtc)},
                 tg.finished_at_utc AS {nameof(TurnGameRow.FinishedAtUtc)},
                 (SELECT COUNT(*)::int
                  FROM dateplanner.turn_game_rounds r
                  WHERE r.turn_game_id = tg.id) AS {nameof(TurnGameRow.RoundsPlayed)}
             FROM dateplanner.turn_games tg
             LEFT JOIN dateplanner.users winner ON winner.id = tg.winner_id
             WHERE tg.player_one_id = @UserId OR tg.player_two_id = @UserId
             ORDER BY tg.created_at_utc DESC;
             """;

        var command = new CommandDefinition(sql, request, cancellationToken: cancellationToken);
        TurnGameRow[] rows = (await connection.QueryAsync<TurnGameRow>(command)).ToArray();

        return rows.Select(row => new TurnGameSummaryResponse(
                row.Id,
                string.IsNullOrWhiteSpace(row.GameType)
                    ? null
                    : Enum.Parse<TurnGameType>(row.GameType),
                Enum.Parse<TurnGamePurpose>(row.Purpose),
                Enum.Parse<TurnGameStatus>(row.Status),
                row.WinnerId,
                row.WinnerName,
                row.CreatedAtUtc,
                row.FinishedAtUtc,
                row.RoundsPlayed))
            .ToArray();
    }

    private sealed class TurnGameRow
    {
        public Guid Id { get; init; }
        public string? GameType { get; init; }
        public string Purpose { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public Guid? WinnerId { get; init; }
        public string? WinnerName { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateTime? FinishedAtUtc { get; init; }
        public int RoundsPlayed { get; init; }
    }
}

using System.Data.Common;
using Dapper;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using MediatR;

namespace DatePlanner.Application.TurnGames.GetCurrentScheduledTurnGame;

public sealed class GetCurrentScheduledTurnGameQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IRequestHandler<GetCurrentScheduledTurnGameQuery, Result<CurrentScheduledTurnGameResponse>>
{
    public async Task<Result<CurrentScheduledTurnGameResponse>> Handle(
        GetCurrentScheduledTurnGameQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 tg.id AS {nameof(TurnGameRow.Id)},
                 tg.game_type AS {nameof(TurnGameRow.GameType)},
                 tg.status AS {nameof(TurnGameRow.Status)},
                 tg.created_at_utc AS {nameof(TurnGameRow.CreatedAtUtc)}
             FROM dateplanner.turn_games tg
             INNER JOIN dateplanner.couples c ON c.id = tg.couple_id AND c.is_active
             WHERE tg.purpose = 'PlanningDecision'
               AND tg.status <> 'Finished'
               AND (tg.player_one_id = @UserId OR tg.player_two_id = @UserId)
             ORDER BY tg.created_at_utc DESC
             LIMIT 1;
             """;

        var command = new CommandDefinition(sql, request, cancellationToken: cancellationToken);
        TurnGameRow? row = await connection.QuerySingleOrDefaultAsync<TurnGameRow>(command);
        ScheduledTurnGameResponse? game = row is null
            ? null
            : new ScheduledTurnGameResponse(
                row.Id,
                string.IsNullOrWhiteSpace(row.GameType)
                    ? null
                    : Enum.Parse<TurnGameType>(row.GameType),
                Enum.Parse<TurnGameStatus>(row.Status),
                row.CreatedAtUtc);

        return Result.Success(new CurrentScheduledTurnGameResponse(game));
    }

    private sealed class TurnGameRow
    {
        public Guid Id { get; init; }
        public string? GameType { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAtUtc { get; init; }
    }
}

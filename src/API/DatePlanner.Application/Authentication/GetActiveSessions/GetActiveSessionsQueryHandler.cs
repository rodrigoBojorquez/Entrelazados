using System.Data.Common;
using Dapper;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using MediatR;

namespace DatePlanner.Application.Authentication.GetActiveSessions;

public sealed class GetActiveSessionsQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    TimeProvider timeProvider)
    : IRequestHandler<GetActiveSessionsQuery, Result<IReadOnlyCollection<ActiveSessionResponse>>>
{
    public async Task<Result<IReadOnlyCollection<ActiveSessionResponse>>> Handle(
        GetActiveSessionsQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 id AS {nameof(ActiveSessionResponse.Id)},
                 family_id AS {nameof(ActiveSessionResponse.FamilyId)},
                 created_at_utc AS {nameof(ActiveSessionResponse.CreatedAtUtc)},
                 expires_at_utc AS {nameof(ActiveSessionResponse.ExpiresAtUtc)}
             FROM dateplanner.refresh_tokens
             WHERE user_id = @UserId
               AND revoked_at_utc IS NULL
               AND expires_at_utc > @UtcNow
             ORDER BY created_at_utc DESC
             """;

        var parameters = new
        {
            request.UserId,
            UtcNow = timeProvider.GetUtcNow().UtcDateTime
        };
        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        ActiveSessionResponse[] sessions =
            (await connection.QueryAsync<ActiveSessionResponse>(command)).ToArray();

        return sessions;
    }
}

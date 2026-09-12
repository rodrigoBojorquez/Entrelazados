using System.Data.Common;
using Dapper;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using MediatR;

namespace DatePlanner.Application.Couples.GetCurrentPartner;

public sealed class GetCurrentPartnerQueryHandler(IDbConnectionFactory dbConnectionFactory) 
    : IRequestHandler<GetCurrentPartnerQuery, Result<GetCurrentPartnerResponse>>
{
    public async Task<Result<GetCurrentPartnerResponse>> Handle(GetCurrentPartnerQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
                SELECT
                    u.id AS {nameof(GetCurrentPartnerResponse.PartnerId)},
                    u.first_name || ' ' || u.last_name AS {nameof(GetCurrentPartnerResponse.PartnerName)},
                    COALESCE(dp.favorite_activities, ARRAY[]::text[]) AS {nameof(PartnerRow.FavoriteActivities)},
                    COALESCE(dp.avoided_activities, ARRAY[]::text[]) AS {nameof(PartnerRow.AvoidedActivities)},
                    COALESCE(dp.food_preferences, ARRAY[]::text[]) AS {nameof(PartnerRow.FoodPreferences)}
                FROM dateplanner.couples c
                JOIN dateplanner.users u
                    ON u.id = CASE
                        WHEN c.partner_one_id = @UserId THEN c.partner_two_id
                        ELSE c.partner_one_id
                    END
                LEFT JOIN dateplanner.dating_preferences dp 
                    ON dp.user_id = u.id
                WHERE c.is_active
                  AND (c.partner_one_id = @UserId OR c.partner_two_id = @UserId);
             """;

        var command = new CommandDefinition(sql, request, cancellationToken: cancellationToken);
        PartnerRow? partner = await connection.QuerySingleOrDefaultAsync<PartnerRow>(command);

        if (partner is null)
        {
            return Result.Failure<GetCurrentPartnerResponse>(CoupleErrors.PartnerNotFound(request.UserId));
        }

        return new GetCurrentPartnerResponse(
            partner.PartnerId,
            partner.PartnerName,
            new PartnerPreferencesResponse(
                partner.FavoriteActivities,
                partner.AvoidedActivities,
                partner.FoodPreferences));
    }

    private sealed record PartnerRow(
        Guid PartnerId,
        string PartnerName,
        string[] FavoriteActivities,
        string[] AvoidedActivities,
        string[] FoodPreferences);
}

using System.Data.Common;
using Dapper;
using DatePlanner.Application.Common.Data;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Users;
using MediatR;

namespace DatePlanner.Application.Users.GetUserProfile;

public sealed class GetUserProfileQueryHandler(IDbConnectionFactory dbConnectionFactory)
    : IRequestHandler<GetUserProfileQuery, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            $"""
             SELECT
                 u.id AS {nameof(UserProfileRow.Id)},
                 u.email AS {nameof(UserProfileRow.Email)},
                 u.first_name AS {nameof(UserProfileRow.FirstName)},
                 u.last_name AS {nameof(UserProfileRow.LastName)},
                 u.partner_code AS {nameof(UserProfileRow.PartnerCode)},
                 COALESCE(dp.favorite_activities, ARRAY[]::text[]) AS {nameof(UserProfileRow.FavoriteActivities)},
                 COALESCE(dp.avoided_activities, ARRAY[]::text[]) AS {nameof(UserProfileRow.AvoidedActivities)},
                 COALESCE(dp.food_preferences, ARRAY[]::text[]) AS {nameof(UserProfileRow.FoodPreferences)}
             FROM dateplanner.users u
             LEFT JOIN dateplanner.dating_preferences dp ON dp.user_id = u.id
             WHERE u.id = @UserId
             """;

        var command = new CommandDefinition(sql, request, cancellationToken: cancellationToken);
        UserProfileRow? user = await connection.QuerySingleOrDefaultAsync<UserProfileRow>(command);

        if (user is null)
        {
            return Result.Failure<UserProfileResponse>(UserErrors.NotFound(request.UserId));
        }

        return new UserProfileResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            $"{user.FirstName} {user.LastName}",
            user.PartnerCode,
            new DatingPreferencesResponse(
                user.FavoriteActivities,
                user.AvoidedActivities,
                user.FoodPreferences));
    }

    private sealed class UserProfileRow
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string PartnerCode { get; init; } = string.Empty;
        public string[] FavoriteActivities { get; init; } = [];
        public string[] AvoidedActivities { get; init; } = [];
        public string[] FoodPreferences { get; init; } = [];
    }
}

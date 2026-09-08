using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Users;
using MediatR;

namespace DatePlanner.Application.Users.GetUserProfile;

public sealed class GetUserProfileQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserProfileQuery, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetUserByIdAsync(request.UserId);

        if (user is null)
        {
            return Result.Failure<UserProfileResponse>(UserErrors.NotFound(request.UserId));
        }

        DatingPreferences? preferences = user.DatingPreferences;

        return new UserProfileResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.FullName,
            user.PartnerCode,
            new DatingPreferencesResponse(
                preferences?.FavoriteActivities.ToArray() ?? [],
                preferences?.AvoidedActivities.ToArray() ?? [],
                preferences?.FoodPreferences.ToArray() ?? []));
    }
}

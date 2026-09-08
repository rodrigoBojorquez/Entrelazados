namespace DatePlanner.Application.Users.GetUserProfile;

public sealed record DatingPreferencesResponse(
    IReadOnlyCollection<string> FavoriteActivities,
    IReadOnlyCollection<string> AvoidedActivities,
    IReadOnlyCollection<string> FoodPreferences);
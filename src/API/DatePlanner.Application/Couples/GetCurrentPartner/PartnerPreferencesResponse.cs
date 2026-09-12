namespace DatePlanner.Application.Couples.GetCurrentPartner;

public sealed record PartnerPreferencesResponse(
    IReadOnlyCollection<string> FavoriteActivities,
    IReadOnlyCollection<string> AvoidedActivities,
    IReadOnlyCollection<string> FoodPreferences);
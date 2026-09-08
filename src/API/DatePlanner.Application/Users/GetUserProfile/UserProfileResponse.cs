namespace DatePlanner.Application.Users.GetUserProfile;

public sealed record UserProfileResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string PartnerCode,
    DatingPreferencesResponse DatingPreferences);
using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.Users.GetUserProfile;

public sealed record GetUserProfileQuery(Guid UserId) : IRequest<Result<UserProfileResponse>>;

internal sealed class GetUserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
    public GetUserProfileQueryValidator() => RuleFor(query => query.UserId).NotEmpty();
}

public sealed record UserProfileResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string PartnerCode,
    DatingPreferencesResponse DatingPreferences);

public sealed record DatingPreferencesResponse(
    IReadOnlyCollection<string> FavoriteActivities,
    IReadOnlyCollection<string> AvoidedActivities,
    IReadOnlyCollection<string> FoodPreferences);

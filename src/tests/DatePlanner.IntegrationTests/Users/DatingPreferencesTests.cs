using DatePlanner.Application.Users.GetUserProfile;
using DatePlanner.Application.Users.UpdateDatingPreferences;
using DatePlanner.Domain.Abstractions;
using DatePlanner.IntegrationTests.Abstractions;
using FluentAssertions;

namespace DatePlanner.IntegrationTests.Users;

public sealed class DatingPreferencesTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task DatingPreferences_ShouldSupportCompleteAddAndRemoveWorkflow()
    {
        RegisteredUser user = await RegisterUserAsync();

        Result addFavoriteResult = await Sender.Send(
            new AddFavoriteActivityCommand(user.Id, "  Hiking  "));
        Result addAvoidedResult = await Sender.Send(
            new AddAvoidedActivityCommand(user.Id, "  Karaoke  "));
        Result addFoodResult = await Sender.Send(
            new AddFoodPreferenceCommand(user.Id, "  Sushi  "));

        addFavoriteResult.IsSuccess.Should().BeTrue();
        addAvoidedResult.IsSuccess.Should().BeTrue();
        addFoodResult.IsSuccess.Should().BeTrue();

        Result<UserProfileResponse> addedProfile = await Sender.Send(
            new GetUserProfileQuery(user.Id));

        addedProfile.Value.DatingPreferences.FavoriteActivities.Should().ContainSingle("Hiking");
        addedProfile.Value.DatingPreferences.AvoidedActivities.Should().ContainSingle("Karaoke");
        addedProfile.Value.DatingPreferences.FoodPreferences.Should().ContainSingle("Sushi");

        Result removeFavoriteResult = await Sender.Send(
            new RemoveFavoriteActivityCommand(user.Id, "Hiking"));
        Result removeAvoidedResult = await Sender.Send(
            new RemoveAvoidedActivityCommand(user.Id, "Karaoke"));
        Result removeFoodResult = await Sender.Send(
            new RemoveFoodPreferenceCommand(user.Id, "Sushi"));

        removeFavoriteResult.IsSuccess.Should().BeTrue();
        removeAvoidedResult.IsSuccess.Should().BeTrue();
        removeFoodResult.IsSuccess.Should().BeTrue();

        Result<UserProfileResponse> removedProfile = await Sender.Send(
            new GetUserProfileQuery(user.Id));

        removedProfile.Value.DatingPreferences.FavoriteActivities.Should().BeEmpty();
        removedProfile.Value.DatingPreferences.AvoidedActivities.Should().BeEmpty();
        removedProfile.Value.DatingPreferences.FoodPreferences.Should().BeEmpty();
    }

    [Fact]
    public async Task AddAvoidedActivity_ShouldFail_WhenActivityIsAlreadyFavorite()
    {
        RegisteredUser user = await RegisterUserAsync();
        await Sender.Send(new AddFavoriteActivityCommand(user.Id, "Cinema"));

        Result result = await Sender.Send(
            new AddAvoidedActivityCommand(user.Id, "Cinema"));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("DatingPreferences.FavoriteActivityCannotBeAvoided");
    }
}

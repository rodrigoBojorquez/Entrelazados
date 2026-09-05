using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Users;
using DatePlanner.UnitTests.Abstractions;
using FluentAssertions;

namespace DatePlanner.UnitTests.Users;

public class DatingPreferencesTests
{
    private readonly DatingPreferences _preferences = DatingPreferences.Create(Guid.NewGuid());

    [Fact]
    public void Create_ShouldInitializeEmptyPreferenceCollections_ForUser()
    {
        Guid userId = Guid.NewGuid();

        DatingPreferences preferences = DatingPreferences.Create(userId);

        preferences.Id.Should().NotBeEmpty();
        preferences.UserId.Should().Be(userId);
        preferences.FavoriteActivities.Should().BeEmpty();
        preferences.AvoidedActivities.Should().BeEmpty();
        preferences.FoodPreferences.Should().BeEmpty();
    }

    [Fact]
    public void AddFavoriteActivity_ShouldAddExactlyOneActivity_WhenNew()
    {
        Result result = _preferences.AddFavoriteActivity("hiking");

        result.IsSuccess.Should().BeTrue();
        _preferences.FavoriteActivities.Should().ContainSingle().Which.Should().Be("hiking");
    }

    [Fact]
    public void AddFavoriteActivity_ShouldRejectDuplicate_WithoutChangingCollection()
    {
        _preferences.AddFavoriteActivity("hiking");

        Result result = _preferences.AddFavoriteActivity("hiking");

        result.IsFailure.Should().BeTrue();
        result.Error.ShouldHaveCode("DatingPreferences.ActivityAlreadyInTheList", ErrorType.Problem);
        _preferences.FavoriteActivities.Should().ContainSingle("hiking");
    }

    [Fact]
    public void AddFavoriteActivity_ShouldRejectActivityAlreadyAvoided_WithoutChangingEitherList()
    {
        _preferences.AddAvoidedActivity("karaoke");

        Result result = _preferences.AddFavoriteActivity("karaoke");

        result.Error.ShouldHaveCode("DatingPreferences.AvoidedActivityCannotBeFavorite", ErrorType.Problem);
        _preferences.FavoriteActivities.Should().BeEmpty();
        _preferences.AvoidedActivities.Should().ContainSingle("karaoke");
    }

    [Fact]
    public void RemoveFavoriteActivity_ShouldRemoveOnlyRequestedActivity_WhenPresent()
    {
        _preferences.AddFavoriteActivity("hiking");
        _preferences.AddFavoriteActivity("cooking");

        Result result = _preferences.RemoveFavoriteActivity("hiking");

        result.IsSuccess.Should().BeTrue();
        _preferences.FavoriteActivities.Should().Equal("cooking");
    }

    [Fact]
    public void RemoveFavoriteActivity_ShouldFailWithoutChangingCollection_WhenMissing()
    {
        _preferences.AddFavoriteActivity("hiking");

        Result result = _preferences.RemoveFavoriteActivity("cooking");

        result.Error.ShouldHaveCode("DatingPreferences.ActivityNotFoundInTheList", ErrorType.Problem);
        _preferences.FavoriteActivities.Should().Equal("hiking");
    }

    [Fact]
    public void AddAvoidedActivity_ShouldRejectActivityAlreadyFavorite_WithoutChangingEitherList()
    {
        _preferences.AddFavoriteActivity("dancing");

        Result result = _preferences.AddAvoidedActivity("dancing");

        result.Error.ShouldHaveCode("DatingPreferences.FavoriteActivityCannotBeAvoided", ErrorType.Problem);
        _preferences.AvoidedActivities.Should().BeEmpty();
        _preferences.FavoriteActivities.Should().ContainSingle("dancing");
    }

    [Fact]
    public void AddAvoidedActivity_ShouldRejectDuplicate_WithoutChangingCollection()
    {
        _preferences.AddAvoidedActivity("crowds");

        Result result = _preferences.AddAvoidedActivity("crowds");

        result.Error.ShouldHaveCode("DatingPreferences.ActivityAlreadyInTheList", ErrorType.Problem);
        _preferences.AvoidedActivities.Should().ContainSingle("crowds");
    }

    [Fact]
    public void RemoveAvoidedActivity_ShouldRemoveOnlyRequestedActivity_WhenPresent()
    {
        _preferences.AddAvoidedActivity("crowds");
        _preferences.AddAvoidedActivity("heights");

        Result result = _preferences.RemoveAvoidedActivity("crowds");

        result.IsSuccess.Should().BeTrue();
        _preferences.AvoidedActivities.Should().Equal("heights");
    }

    [Fact]
    public void RemoveAvoidedActivity_ShouldFailWithoutChangingCollection_WhenMissing()
    {
        _preferences.AddAvoidedActivity("crowds");

        Result result = _preferences.RemoveAvoidedActivity("heights");

        result.Error.ShouldHaveCode("DatingPreferences.ActivityNotFoundInTheList", ErrorType.Problem);
        _preferences.AvoidedActivities.Should().Equal("crowds");
    }

    [Fact]
    public void AddFoodPreference_ShouldRejectDuplicate_WithoutChangingCollection()
    {
        _preferences.AddFoodPreferences("sushi");

        Result result = _preferences.AddFoodPreferences("sushi");

        result.Error.ShouldHaveCode("DatingPreferences.ActivityAlreadyInTheList", ErrorType.Problem);
        _preferences.FoodPreferences.Should().ContainSingle("sushi");
    }

    [Fact]
    public void RemoveFoodPreference_ShouldRemoveRequestedFood_WhenPresent()
    {
        _preferences.AddFoodPreferences("sushi");
        _preferences.AddFoodPreferences("tacos");

        Result result = _preferences.RemoveFoodPreferences("sushi");

        result.IsSuccess.Should().BeTrue();
        _preferences.FoodPreferences.Should().Equal("tacos");
    }

    [Fact]
    public void RemoveFoodPreference_ShouldFailWithoutChangingCollection_WhenMissing()
    {
        _preferences.AddFoodPreferences("sushi");

        Result result = _preferences.RemoveFoodPreferences("tacos");

        result.Error.ShouldHaveCode("DatingPreferences.ActivityNotFoundInTheList", ErrorType.Problem);
        _preferences.FoodPreferences.Should().Equal("sushi");
    }
}

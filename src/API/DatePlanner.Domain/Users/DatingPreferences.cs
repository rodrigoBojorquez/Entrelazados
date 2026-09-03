using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Users;

public class DatingPreferences : Entity
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    private readonly List<string> _favoriteActivities = [];

    public IReadOnlyCollection<string> FavoriteActivities => _favoriteActivities;

    private readonly List<string> _avoidedActivities = [];

    public IReadOnlyCollection<string> AvoidedActivities => _avoidedActivities;

    private readonly List<string> _foodPreferences = [];

    // Always positive preferences
    public IReadOnlyCollection<string> FoodPreferences => _foodPreferences;

    private DatingPreferences()
    {
    }

    public static DatingPreferences Create(Guid userId)
    {
        return new DatingPreferences
        {
            Id = Guid.NewGuid(),
            UserId = userId
        };
    }
    
    public Result AddFavoriteActivity(string activity)
    {
        if (_avoidedActivities.Contains(activity))
        {
            return Result.Failure(DatingPreferencesErrors.AvoidedActivityCannotBeFavorite(activity));
        }

        if (_favoriteActivities.Contains(activity))
        {
            return Result.Failure(DatingPreferencesErrors.ActivityAlreadyInTheList(activity));
        }
        
        _favoriteActivities.Add(activity);
        return Result.Success();
    }

    public Result RemoveFavoriteActivity(string activity)
    {
        var eliminated = _favoriteActivities.Remove(activity);

        return eliminated ? 
            Result.Success() : 
            Result.Failure(DatingPreferencesErrors.ActivityNotFoundInTheList(activity));
    }
    
    public Result AddAvoidedActivity(string activity)
    {
        if (_favoriteActivities.Contains(activity))
        {
            return Result.Failure(DatingPreferencesErrors.FavoriteActivityCannotBeAvoided(activity));
        }

        if (_avoidedActivities.Contains(activity))
        {
            return Result.Failure(DatingPreferencesErrors.ActivityAlreadyInTheList(activity));
        }
        
        _avoidedActivities.Add(activity);
        return Result.Success();
    }

    public Result RemoveAvoidedActivity(string activity)
    {
        var eliminated  = _avoidedActivities.Remove(activity);
        return eliminated ?
            Result.Success() :
            Result.Failure(DatingPreferencesErrors.ActivityNotFoundInTheList(activity));
    }

    public Result AddFoodPreferences(string food)
    {
        if (_foodPreferences.Contains(food))
        {
            return Result.Failure(DatingPreferencesErrors.ActivityAlreadyInTheList(food));
        }
        
        _foodPreferences.Add(food);
        return Result.Success();
    }

    public Result RemoveFoodPreferences(string food)
    {
        var eliminated = _foodPreferences.Remove(food);
        return eliminated ?
            Result.Success() :
            Result.Failure(DatingPreferencesErrors.ActivityNotFoundInTheList(food));
    }
}
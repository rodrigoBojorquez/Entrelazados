using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Users;

public static class DatingPreferencesErrors
{
    public static Error AvoidedActivityCannotBeFavorite(string activity) =>
        Error.Problem("DatingPreferences.AvoidedActivityCannotBeFavorite",
            $"{activity} está marcada como una actividad a evitar...");
    
    public static Error FavoriteActivityCannotBeAvoided(string activity) =>
        Error.Problem("DatingPreferences.FavoriteActivityCannotBeAvoided",
            $"{activity} está marcada como una actividad favorita...");
    
    public static Error ActivityAlreadyInTheList(string activity) =>
        Error.Problem("DatingPreferences.ActivityAlreadyInTheList",
            $"{activity} ya está en la lista...");
    
    public static Error ActivityNotFoundInTheList(string activity) =>
        Error.Problem("DatingPreferences.ActivityNotFoundInTheList",
            $"{activity} no se encuentra en la lista...");
}
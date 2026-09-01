using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public static class LocationErrors
{
    public static Error InvalidLatitudeRange(decimal value) => 
        Error.Problem(
            "Location.InvalidLatitudeRange", 
            $"El valor {value} no es un rango válido para latitud");
    
    public static Error InvalidLongitudeRange(decimal value) =>
        Error.Problem(
            "Location.InvalidLongitudeRange", 
            $"El valor {value} no es un rango válido para longitud");
}
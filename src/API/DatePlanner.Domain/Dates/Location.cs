using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public sealed record Location
{
    public decimal Latitude { get; }
    public decimal Longitude { get; }

    private Location(decimal latitude, decimal longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static Result<Location> Create(
        decimal latitude,
        decimal longitude)
    {
        if (latitude is < -90 or > 90)
            return Result.Failure<Location>(LocationErrors.InvalidLatitudeRange(latitude));

        if (longitude is < -180 or > 180)
            return Result.Failure<Location>(LocationErrors.InvalidLongitudeRange(longitude));

        return new Location(latitude, longitude);
    }
}
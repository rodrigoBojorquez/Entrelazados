using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public class ItineraryItem
{
    public Guid Id { get; private set; }

    public int Order { get; private set; }

    public string Name { get; set; }

    public TimeOnly? StartsAt { get; set; }

    public Location? Location { get; set; }

    private ItineraryItem()
    {
    }

    // public static Result<ItineraryItem> Create(string name, TimeOnly start)
    // {
    //     ItineraryItem item = new ItineraryItem
    //     {
    //         Name = name,
    //         StartsAt = start,
    //         EndsAt = end
    //     };
    //
    //     return item;
    // }
}

public record Location(
    string Name,
    decimal Latitude,
    decimal Longitude);
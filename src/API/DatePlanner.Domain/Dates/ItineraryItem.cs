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

    public static ItineraryItem Create(
        int order, 
        string name,
        Location? location,
        TimeOnly? startsAt)
    {
        ItineraryItem item = new ItineraryItem
        {
            Id = Guid.NewGuid(),
            Order = order,
            Name = name,
            StartsAt = startsAt,
            Location = location
        };

        return item;
    }
}
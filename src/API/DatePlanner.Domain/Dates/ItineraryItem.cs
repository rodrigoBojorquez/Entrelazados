using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public sealed class ItineraryItem : Entity
{
    public Guid Id { get; private set; }

    public int Order { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public TimeOnly? StartsAt { get; private set; }

    public Location? Location { get; private set; }

    private ItineraryItem()
    {
    }

    internal static ItineraryItem Create(
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

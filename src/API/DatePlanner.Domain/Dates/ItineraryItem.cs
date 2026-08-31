using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public class ItineraryItem
{
    // Name is the Identifier
    public string Name  { get; set; }
    
    public TimeOnly StartsAt  { get; set; }
    
    public TimeOnly EndsAt  { get; set; }
    
    private ItineraryItem() {}

    public static Result<ItineraryItem> Create(string name, TimeOnly start, TimeOnly end)
    {
        if (start >= end)
        {
            return Result.Failure<ItineraryItem>(DateErrors.EndTimePrecedesStartTime);
        }
        
        ItineraryItem item = new ItineraryItem
        {
            Name = name,
            StartsAt = start,
            EndsAt = end
        };

        return item;
    }
}
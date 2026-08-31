using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public class Date : Entity
{
    private Date() {}
    
    public Guid Id { get; private set; }
    
    public string Title { get; private set; }
    
    public DateOnly ScheduledFor { get; private set; }
    
    public DateStatus Status { get; private set; }
    
    public List<ItineraryItem> Itinerary { get; private set; }
    
    public static Result<Date> Create(
        string title, 
        DateOnly scheduledFor, 
        (string Name, TimeOnly StartsAt, TimeOnly EndsAt)[] itinerary)
    {
        List<ItineraryItem> itineraryItems = [];
        
        foreach (var item in itinerary)
        {
            var result = ItineraryItem.Create(item.Name, item.StartsAt, item.EndsAt);
            
            if (result.IsFailure)
            {
                return Result.Failure<Date>(result.Error);
            }
            
            itineraryItems.Add(result.Value);
        }
        
        Date date = new Date
        {
            Id = Guid.NewGuid(),
            Title = title,
            Itinerary =  itineraryItems,
            ScheduledFor = scheduledFor,
            Status = DateStatus.Proposed
        };

        return Result.Success(date);
    }

    public void Accept()
    {
        Status = DateStatus.Booked;
    }

    public void Cancel()
    {
        Status = DateStatus.Canceled;
    }

    public void Attend()
    {
        Status = DateStatus.Attended;
    }
}
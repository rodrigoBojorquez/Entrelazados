using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public class Date : Entity
{
    private Date() {}

    public static Date Create()
    {
        Date date = new Date
        {

        };

        return date;
    }
    
    public Guid Id { get; private set; }
    
    public string Title { get; private set; }
    
    public DateOnly ScheduledFor { get; private set; }
    
    public DateStatus Status { get; private set; }
    
    public List<ItineraryItem> Itinerary { get; private set; }
}

public class ItineraryItem
{
    public Guid ItineraryId { get; set; }
    
    public string Name  { get; set; }
    
    public 
}
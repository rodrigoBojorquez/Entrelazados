using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public class DatePlan : Entity
{
    private DatePlan() {}
    
    public Guid Id { get; private set; }
    
    public Guid CoupleId { get; private set; }
    
    public Guid PartnerWhoPlanedId { get; private set; }
    
    public string Title { get; private set; }
    
    public DateOnly ScheduledFor { get; private set; }
    
    public DateStatus Status { get; private set; }
    
    public List<ItineraryItem> Itinerary { get; private set; }
    
    // public static Result<DatePlan> Create(
    //     string title, 
    //     DateOnly scheduledFor, 
    //     (string Name, TimeOnly StartsAt, TimeOnly EndsAt)[] itinerary)
    // {
    //     List<ItineraryItem> itineraryItems = [];
    //     
    //     foreach (var item in itinerary)
    //     {
    //         var result = ItineraryItem.Create(item.Name, item.StartsAt, item.EndsAt);
    //         
    //         if (result.IsFailure)
    //         {
    //             return Result.Failure<DatePlan>(result.Error);
    //         }
    //         
    //         itineraryItems.Add(result.Value);
    //     }
    //     
    //     DatePlan datePlan = new DatePlan
    //     {
    //         Id = Guid.NewGuid(),
    //         Title = title,
    //         Itinerary =  itineraryItems,
    //         ScheduledFor = scheduledFor,
    //         Status = DateStatus.Proposed
    //     };
    //
    //     return Result.Success(datePlan);
    // }

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
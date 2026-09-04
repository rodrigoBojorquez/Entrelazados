namespace DatePlanner.Infrastructure.DateIdeas;

public class DateIdea
{
    public Guid Id { get; set; }
    
    public Guid CoupleId { get; set; }
    
    public Guid? ResultPlan { get; set; }
    
    public decimal PreferredBudget { get; set; }
    
    public decimal? EstimatedBudget { get; set; }
    
    public string City { get; set; } = string.Empty;
    
    public TimeOnly? StartsAt { get; set; }
    
    public TimeOnly? EndsAt { get; set; }

    public List<DateIdeaItineraryItem> Itinerary { get; set; } = [];

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
}
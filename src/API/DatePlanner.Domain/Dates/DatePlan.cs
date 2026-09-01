using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public class DatePlan : Entity
{
    private DatePlan()
    {
    }

    public Guid Id { get; private set; }

    public Guid CoupleId { get; private set; }

    public Guid PartnerWhoPlanedId { get; private set; }

    public string Title { get; private set; }

    public DateOnly ScheduledFor { get; private set; }

    public DateStatus Status { get; private set; }

    public List<ItineraryItem> Itinerary { get; private set; } = [];

    public static DatePlan Create(
        Guid coupleId,
        Guid partnerWhoPlanedId,
        string title,
        DateOnly scheduledFor)
    {
        DatePlan datePlan = new DatePlan
        {
            Id = Guid.NewGuid(),
            Title = title,
            CoupleId = coupleId,
            PartnerWhoPlanedId = partnerWhoPlanedId,
            ScheduledFor = scheduledFor,
            Status = DateStatus.Booked
        };

        return datePlan;
    }

    public void AddItineraryItem(
        string name,
        Location? location,
        TimeOnly? startsAt)
    {
        Itinerary.Add(ItineraryItem.Create(Itinerary.Count + 1, name, location, startsAt));
    }

    public void RemoveItineraryItem(Guid itineraryItemId)
    {
        var item = Itinerary.FirstOrDefault(x => x.Id == itineraryItemId);

        if (item is not null)
            Itinerary.Remove(item);
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
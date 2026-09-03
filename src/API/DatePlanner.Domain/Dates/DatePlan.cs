using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public class DatePlan : Entity
{
    private DatePlan()
    {
    }

    public Guid Id { get; private set; }

    public Guid CoupleId { get; private set; }

    public Guid PartnerWhoPlannedId { get; private set; }

    public DateOnly? ScheduledFor { get; private set; }

    public DateStatus Status { get; private set; }

    private readonly List<ItineraryItem> _itinerary = [];

    public IReadOnlyCollection<ItineraryItem> Itinerary => _itinerary.AsReadOnly();

    public static Result<DatePlan> Create(
        Guid coupleId,
        Guid partnerWhoPlannedId)
    {
        DatePlan datePlan = new DatePlan
        {
            Id = Guid.NewGuid(),
            CoupleId = coupleId,
            PartnerWhoPlannedId = partnerWhoPlannedId,
            Status = DateStatus.Planning
        };

        return Result.Success(datePlan);
    }

    public Result AddItineraryItem(
        string name,
        Location? location,
        TimeOnly? startsAt)
    {
        if (Status != DateStatus.Planning)
        {
            return Result.Failure(DatePlanErrors.MustBePlanning);
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(DatePlanErrors.ItineraryItemNameIsRequired);
        }

        int nextOrder = _itinerary.Count == 0
            ? 1
            : _itinerary.Max(x => x.Order) + 1;

        _itinerary.Add(ItineraryItem.Create(
            nextOrder,
            name.Trim(),
            location,
            startsAt));

        return Result.Success();
    }

    public Result RemoveItineraryItem(Guid itineraryItemId)
    {
        if (Status != DateStatus.Planning)
        {
            return Result.Failure(DatePlanErrors.MustBePlanning);
        }

        if (itineraryItemId == Guid.Empty)
        {
            return Result.Failure(DatePlanErrors.ItineraryItemIdIsRequired);
        }

        ItineraryItem? item = _itinerary.FirstOrDefault(x => x.Id == itineraryItemId);

        if (item is null)
        {
            return Result.Failure(DatePlanErrors.ItineraryItemNotFound(itineraryItemId));
        }

        _itinerary.Remove(item);

        return Result.Success();
    }

    public Result Schedule(DateOnly scheduledFor)
    {
        if (Status != DateStatus.Planning)
        {
            return Result.Failure(DatePlanErrors.MustBePlanning);
        }

        if (scheduledFor == default)
        {
            return Result.Failure(DatePlanErrors.ScheduledDateIsRequired);
        }

        if (_itinerary.Count == 0)
        {
            return Result.Failure(DatePlanErrors.EmptyItinerary);
        }

        ScheduledFor = scheduledFor;
        Status = DateStatus.Booked;

        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status is DateStatus.Canceled or DateStatus.Attended)
        {
            return Result.Failure(DatePlanErrors.CannotCancel);
        }

        Status = DateStatus.Canceled;

        return Result.Success();
    }

    public Result Attend()
    {
        if (Status != DateStatus.Booked)
        {
            return Result.Failure(DatePlanErrors.MustBeBookedToAttend);
        }

        Status = DateStatus.Attended;

        return Result.Success();
    }
}

using DatePlanner.Domain.Dates;

namespace DatePlanner.Infrastructure.DateIdeas;

public record DateIdeaItineraryItem(
    Guid Id,
    int Order,
    string Name,
    List<string> Tags,
    TimeOnly? StartsAt,
    Location? Location)
{
    private DateIdeaItineraryItem()
        : this(Guid.Empty, 0, string.Empty, [], null, null)
    {
    }
}

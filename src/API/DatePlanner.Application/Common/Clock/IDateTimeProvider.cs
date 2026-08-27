namespace DatePlanner.Application.Common.Clock;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

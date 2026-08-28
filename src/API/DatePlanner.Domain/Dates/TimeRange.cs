using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public class TimeRange
{
    public TimeOnly Start { get; private set; }
    public TimeOnly End { get; private set; }
    
    private TimeRange() {}

    public Result<TimeRange> Create(TimeOnly start, TimeOnly end)
    {
        if (end <= start)
        {
            return Result.Failure<TimeRange>(TimeRangeErrors.EndTimePrecedesStartTime);
        }
        
        return Result.Success(new TimeRange
        {
            Start = start,
            End = end
        });
    }
}

public static class TimeRangeErrors
{
    public static Error
        EndTimePrecedesStartTime => Error.Problem(
        "TimeRange.EndTimePrecedesStartTime", 
        "La hora de fin es menor a la hora de inicio");
}
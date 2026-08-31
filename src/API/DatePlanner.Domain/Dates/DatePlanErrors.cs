using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Dates;

public static class DatePlanErrors
{
    public static Error
        EndTimePrecedesStartTime => Error.Problem(
        "TimeRange.EndTimePrecedesStartTime", 
        "La hora de fin es menor a la hora de inicio");
}
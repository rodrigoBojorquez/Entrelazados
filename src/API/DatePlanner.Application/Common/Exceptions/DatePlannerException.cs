using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Application.Common.Exceptions;

public sealed class DatePlannerException : Exception
{
    public DatePlannerException(string requestName, Error? error = default, Exception? innerException = default)
        : base("Application exception", innerException)
    {
        RequestName = requestName;
        Error = error;
    }

    public string RequestName { get; }

    public Error? Error { get; }
}

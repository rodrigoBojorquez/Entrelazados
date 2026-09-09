using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Api.Endpoints.TurnGames;

public sealed record TurnGameHubResponse<T>(T? Value, TurnGameHubError? Error)
{
    public bool IsSuccess => Error is null;

    internal static TurnGameHubResponse<T> Success(T value) => new(value, null);

    internal static TurnGameHubResponse<T> Failure(Result result)
    {
        IReadOnlyCollection<TurnGameHubValidationError>? validationErrors =
            result.Error is ValidationError validationError
                ? validationError.Errors
                    .Select(error => new TurnGameHubValidationError(error.Code, error.Description))
                    .ToArray()
                : null;

        return new TurnGameHubResponse<T>(
            default,
            new TurnGameHubError(
                result.Error.Code,
                result.Error.Description,
                result.Error.Type.ToString(),
                validationErrors));
    }
}

public sealed record TurnGameHubError(
    string Code,
    string Description,
    string Type,
    IReadOnlyCollection<TurnGameHubValidationError>? ValidationErrors);

public sealed record TurnGameHubValidationError(string Code, string Description);

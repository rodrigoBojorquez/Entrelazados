using FluentValidation;

namespace DatePlanner.Application.TurnGames.GetCurrentScheduledTurnGame;

internal sealed class GetCurrentScheduledTurnGameQueryValidator
    : AbstractValidator<GetCurrentScheduledTurnGameQuery>
{
    public GetCurrentScheduledTurnGameQueryValidator() =>
        RuleFor(query => query.UserId).NotEmpty();
}

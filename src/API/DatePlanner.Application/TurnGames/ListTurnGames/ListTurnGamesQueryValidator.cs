using FluentValidation;

namespace DatePlanner.Application.TurnGames.ListTurnGames;

internal sealed class ListTurnGamesQueryValidator : AbstractValidator<ListTurnGamesQuery>
{
    public ListTurnGamesQueryValidator() => RuleFor(query => query.UserId).NotEmpty();
}

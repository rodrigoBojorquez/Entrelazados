using FluentValidation;

namespace DatePlanner.Application.TurnGames.GetTurnGame;

internal sealed class GetTurnGameQueryValidator : AbstractValidator<GetTurnGameQuery>
{
    public GetTurnGameQueryValidator()
    {
        RuleFor(query => query.UserId).NotEmpty();
        RuleFor(query => query.TurnGameId).NotEmpty();
    }
}
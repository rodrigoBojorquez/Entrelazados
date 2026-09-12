using FluentValidation;

namespace DatePlanner.Application.TurnGames.SelectTurnGameType;

internal sealed class SelectTurnGameTypeCommandValidator : AbstractValidator<SelectTurnGameTypeCommand>
{
    public SelectTurnGameTypeCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.TurnGameId).NotEmpty();
        RuleFor(command => command.GameType).IsInEnum();
    }
}

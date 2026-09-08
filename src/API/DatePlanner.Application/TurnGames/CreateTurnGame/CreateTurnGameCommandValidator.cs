using FluentValidation;

namespace DatePlanner.Application.TurnGames.CreateTurnGame;

internal sealed class CreateTurnGameCommandValidator : AbstractValidator<CreateTurnGameCommand>
{
    public CreateTurnGameCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.GameType).IsInEnum();
    }
}
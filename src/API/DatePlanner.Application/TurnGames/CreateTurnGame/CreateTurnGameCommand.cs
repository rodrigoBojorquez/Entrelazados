using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.TurnGames.CreateTurnGame;

public sealed record CreateTurnGameCommand(
    Guid UserId,
    TurnGameType GameType) : IRequest<Result<Guid>>;

internal sealed class CreateTurnGameCommandValidator : AbstractValidator<CreateTurnGameCommand>
{
    public CreateTurnGameCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.GameType).IsInEnum();
    }
}

using DatePlanner.Domain.Abstractions;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.TurnGames.StartTurnGameRound;

public sealed record StartTurnGameRoundCommand(
    Guid UserId,
    Guid TurnGameId) : IRequest<Result>;

internal sealed class StartTurnGameRoundCommandValidator : AbstractValidator<StartTurnGameRoundCommand>
{
    public StartTurnGameRoundCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty();
        RuleFor(command => command.TurnGameId).NotEmpty();
    }
}

using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.TurnGames.Rounds;
using FluentValidation;
using MediatR;

namespace DatePlanner.Application.TurnGames.GetTurnGame;

public sealed record GetTurnGameQuery(Guid UserId, Guid TurnGameId)
    : IRequest<Result<TurnGameResponse>>;

internal sealed class GetTurnGameQueryValidator : AbstractValidator<GetTurnGameQuery>
{
    public GetTurnGameQueryValidator()
    {
        RuleFor(query => query.UserId).NotEmpty();
        RuleFor(query => query.TurnGameId).NotEmpty();
    }
}

public sealed record TurnGameResponse(
    Guid Id,
    Guid CoupleId,
    Guid PlayerOneId,
    Guid PlayerTwoId,
    TurnGameType GameType,
    TurnGameStatus Status,
    Guid? WinnerId,
    DateTime CreatedAtUtc,
    DateTime? StartedAtUtc,
    DateTime? FinishedAtUtc,
    IReadOnlyCollection<TurnGameRoundResponse> Rounds);

public sealed record TurnGameRoundResponse(
    Guid Id,
    int Number,
    TurnGameRoundStatus Status,
    Guid? WinnerId,
    DateTime StartedAtUtc,
    DateTime? FinishedAtUtc,
    IReadOnlyCollection<TurnGameMoveResponse> Moves);

public sealed record TurnGameMoveResponse(
    Guid Id,
    Guid PlayerId,
    int Sequence,
    string? Code,
    DateTime PlayedAtUtc);

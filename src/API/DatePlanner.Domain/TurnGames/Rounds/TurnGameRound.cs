using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames.Rules;

namespace DatePlanner.Domain.TurnGames.Rounds;

public class TurnGameRound : Entity
{
    public Guid Id { get; private set; }
    
    public Guid TurnGameId { get; private set; }
    
    public int Number { get; private set; }
    
    public TurnGameRoundStatus Status { get; private set; }
    
    public Guid? WinnerId { get; private set; }
    
    public DateTime StartedAtUtc { get; private set; }
    
    public DateTime? FinishedAtUtc { get; private set; }
    
    private readonly List<TurnGameMove> _moves = [];
    
    public IReadOnlyCollection<TurnGameMove> Moves => _moves;
    
    public bool IsDraw => Status == TurnGameRoundStatus.Finished && WinnerId is null;

    private TurnGameRound()
    {
    }

    internal static TurnGameRound Create(Guid turnGameId, int number)
    {
        return new TurnGameRound
        {
            Id = Guid.NewGuid(),
            TurnGameId = turnGameId,
            Number = number,
            Status = TurnGameRoundStatus.InProgress,
            StartedAtUtc = DateTime.UtcNow
        };
    }

    internal Result Play(
        Guid playerId,
        ITurnGameMove submittedMove,
        TurnGamePlayers players,
        ITurnGameRules rules)
    {
        if (Status != TurnGameRoundStatus.InProgress)
        {
            return Result.Failure(TurnGameErrors.RoundAlreadyFinished);
        }

        if (!players.TryGetPosition(playerId, out PlayerPosition position))
        {
            return Result.Failure(TurnGameErrors.PlayerIsNotParticipant);
        }
        
        string move =  submittedMove.Code;
        
        List<MoveView> currentMoves = CreateMoveViews(players);

        Result validation = rules.ValidateNextMove(position, move, currentMoves);

        if (validation.IsFailure)
        {
            return validation;
        }

        _moves.Add(TurnGameMove.Create(
            Id,
            playerId,
            _moves.Count + 1,
            move));

        RoundDecision decision = rules.EvaluateRound(CreateMoveViews(players));

        if (decision.Kind != RoundDecisionKind.Pending)
        {
            Complete(decision, players);
        }

        return Result.Success();
    }

    internal RoundView ToView(TurnGamePlayers players)
    {
        if (Status != TurnGameRoundStatus.Finished)
        {
            return new RoundView(RoundDecisionKind.Pending, null);
        }

        if (WinnerId is null)
        {
            return new RoundView(RoundDecisionKind.Draw, null);
        }

        if (!players.TryGetPosition(WinnerId.Value, out PlayerPosition winner))
        {
            throw new InvalidOperationException("The persisted round winner is not a participant.");
        }

        return new RoundView(RoundDecisionKind.Winner, winner);
    }
    
    private List<MoveView> CreateMoveViews(
        TurnGamePlayers players)
    {
        return _moves
            .OrderBy(x => x.Sequence)
            .Select(move =>
            {
                if (!players.TryGetPosition(move.PlayerId, out PlayerPosition player))
                {
                    throw new InvalidOperationException("A persisted move belongs to an unknown player.");
                }

                return new MoveView(player, move.Code, move.Sequence);
            })
            .ToList();
    }

    private void Complete(
        RoundDecision decision,
        TurnGamePlayers players)
    {
        if (decision.Kind == RoundDecisionKind.Pending)
        {
            throw new InvalidOperationException("A round cannot finish with a pending result.");
        }

        WinnerId = decision.Kind == RoundDecisionKind.Winner
            ? players.GetId(decision.Winner!.Value)
            : null;

        Status = TurnGameRoundStatus.Finished;
        FinishedAtUtc = DateTime.UtcNow;
    }
};

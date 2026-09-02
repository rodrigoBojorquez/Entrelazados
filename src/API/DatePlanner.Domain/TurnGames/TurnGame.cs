using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

public class TurnGame : Entity
{
    public Guid Id { get; private set; }

    public Guid CoupleId { get; private set; }
    
    public Guid PlayerOneId { get; private set; }
    
    public Guid PlayerTwoId { get; private set; }
    
    public TurnGameType GameType { get; private set; }

    public TurnGameStatus Status { get; private set; }
    
    public Guid? WinnerId { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    
    public DateTime StartedAtUtc { get; private set; }
    
    public DateTime FinishedAtUtc { get; private set; }
    
    private readonly List<TurnGameRound> _rounds = [];

    public bool IsDraw =>
        Status == TurnGameStatus.Finished && WinnerId is null;

    public IReadOnlyCollection<TurnGameRound> Rounds => _rounds;
    
    private TurnGamePlayers Players => new(PlayerOneId, PlayerTwoId);

    private ITurnGameRules Rules => TurnGameRulesCatalog.Get(GameType);
    
    private TurnGame()
    {
    }

    public static Result<TurnGame> Create(
        Guid coupleId, 
        Guid playerOneId, 
        Guid playerTwoId, 
        TurnGameType gameType)
    {
        if (playerOneId == playerTwoId)
        {
            return Result.Failure<TurnGame>(TurnGameErrors.SamePlayerGame);
        }
        
        
        if (!TurnGameRulesCatalog.TryGetCurrent(gameType, out ITurnGameRules rules))
        {
            return Result.Failure<TurnGame>(TurnGameErrors.UnsupportedGameType);
        }
        
        TurnGame turnGame = new TurnGame
        {
            Id = Guid.NewGuid(),
            CoupleId = coupleId,
            PlayerOneId = playerOneId,
            PlayerTwoId = playerTwoId,
            GameType = gameType,
            Status = TurnGameStatus.Ready,
            CreatedAtUtc = DateTime.UtcNow
        };
        
        turnGame.Raise(new TurnGameStartedDomainEvent(turnGame.Id));

        return turnGame;
    }

    public Result StartRound()
    {
        if (Status ==  TurnGameStatus.Finished)
        {
            return Result.Failure(TurnGameErrors.GameAlreadyFinished);
        }

        if (_rounds.Any(x => x.Status == TurnGameRoundStatus.InProgress))
        {
            return Result.Failure(
                TurnGameErrors.ActiveRoundInProgress);
        }

        if (Status == TurnGameStatus.Ready)
        {
            Status = TurnGameStatus.InProgress;
            StartedAtUtc = DateTime.UtcNow;
        }
        
        _rounds.Add(TurnGameRound.Create(Id, _rounds.Count + 1));
        
        Raise(new TurnGameRoundStartedDomainEvent(Id));
        
        return Result.Success();
    }
    
    public Result Play(Guid playerId, ITurnGameMove move)
    {
        if (Status != TurnGameStatus.InProgress)
        {
            return Result.Failure(TurnGameErrors.GameMustBeInProgress);
        }

        if (!Players.TryGetPosition(playerId, out _))
        {
            return Result.Failure(TurnGameErrors.PlayerIsNotParticipant);
        }

        if (move.GameType != GameType)
        {
            return Result.Failure(TurnGameErrors.IncompatibleMove);
        }

        TurnGameRound? activeRound =
            _rounds.SingleOrDefault(
                x => x.Status ==
                     TurnGameRoundStatus.InProgress);

        if (activeRound is null)
        {
            return Result.Failure(TurnGameErrors.NoActiveRound);
        }

        Result playResult = activeRound.Play(
            playerId,
            move,
            Players,
            Rules);

        if (playResult.IsFailure)
        {
            return playResult;
        }

        if (activeRound.Status == TurnGameRoundStatus.Finished)
        {
            ResolveGame();
        }

        return Result.Success();
    }
    
    private void ResolveGame()
    {
        List<RoundView> finishedRounds = _rounds
            .Where(x =>
                x.Status ==
                TurnGameRoundStatus.Finished)
            .OrderBy(x => x.Number)
            .Select(x => x.ToView(Players))
            .ToList();

        RuleDecision decision = Rules.EvaluateGame(finishedRounds);

        if (decision.Kind == RuleDecisionKind.Pending)
        {
            return;
        }

        WinnerId = decision.Kind == RuleDecisionKind.Winner
            ? Players.GetId(decision.Winner!.Value)
            : null;

        Status = TurnGameStatus.Finished;
        FinishedAtUtc = DateTime.UtcNow;
    }
}
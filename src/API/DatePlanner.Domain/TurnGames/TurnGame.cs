using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;

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
    
    public DateTime? StartedAtUtc { get; private set; }
    
    public DateTime? FinishedAtUtc { get; private set; }
    
    private readonly List<TurnGameRound> _rounds = [];

    public IReadOnlyCollection<TurnGameRound> Rounds => _rounds;
    
    private TurnGamePlayers Players => new(PlayerOneId, PlayerTwoId);

    private ITurnGameRules Rules => TurnGameRulesCatalog.Get(GameType);
    
    private TurnGame()
    {
    }

    public static Result<TurnGame> Create(
        Couple couple,
        TurnGameType gameType)
    {
        ArgumentNullException.ThrowIfNull(couple);

        if (!couple.IsActive)
        {
            return Result.Failure<TurnGame>(TurnGameErrors.CoupleMustBeActive);
        }

        if (!TurnGameRulesCatalog.TryGetCurrent(gameType, out ITurnGameRules rules))
        {
            return Result.Failure<TurnGame>(TurnGameErrors.UnsupportedGameType);
        }
        
        TurnGame turnGame = new TurnGame
        {
            Id = Guid.NewGuid(),
            CoupleId = couple.Id,
            PlayerOneId = couple.PartnerOneId,
            PlayerTwoId = couple.PartnerTwoId,
            GameType = gameType,
            Status = TurnGameStatus.Ready,
            CreatedAtUtc = DateTime.UtcNow
        };
        
        turnGame.Raise(new TurnGameCreatedDomainEvent(turnGame.Id));

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
            Raise(new TurnGameStartedDomainEvent(Id));
        }

        TurnGameRound round = TurnGameRound.Create(Id, _rounds.Count + 1);
        _rounds.Add(round);

        Raise(new TurnGameRoundStartedDomainEvent(Id, round.Id, round.Number));
        
        return Result.Success();
    }
    
    public Result Play(Guid playerId, ITurnGameMove move)
    {
        if (Status != TurnGameStatus.InProgress)
        {
            return Result.Failure(TurnGameErrors.GameMustBeInProgress);
        }

        if (move is null)
        {
            return Result.Failure(TurnGameErrors.MoveIsRequired);
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

        GameDecision decision = Rules.EvaluateGame(finishedRounds);

        if (decision is not GameDecision.Won won)
        {
            return;
        }

        Guid winnerId = Players.GetId(won.Winner);

        WinnerId = winnerId;
        Status = TurnGameStatus.Finished;
        FinishedAtUtc = DateTime.UtcNow;

        Raise(new TurnGameFinishedDomainEvent(Id, CoupleId, winnerId));
    }
}

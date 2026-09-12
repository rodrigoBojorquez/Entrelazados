using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.TurnGames.Events;
using DatePlanner.Domain.TurnGames.Rounds;
using DatePlanner.Domain.TurnGames.Rules;

namespace DatePlanner.Domain.TurnGames;

public class TurnGame : Entity
{
    public static readonly TimeSpan TimedRoundDuration = TimeSpan.FromSeconds(10);

    public Guid Id { get; private set; }

    public Guid CoupleId { get; private set; }
    
    public Guid PlayerOneId { get; private set; }
    
    public Guid PlayerTwoId { get; private set; }
    
    public TurnGameType? GameType { get; private set; }

    public TurnGamePurpose Purpose { get; private set; }

    public TurnGameStatus Status { get; private set; }
    
    public Guid? WinnerId { get; private set; }
    
    public DateTime CreatedAtUtc { get; private set; }
    
    public DateTime? StartedAtUtc { get; private set; }
    
    public DateTime? FinishedAtUtc { get; private set; }
    
    private readonly List<TurnGameRound> _rounds = [];

    public IReadOnlyCollection<TurnGameRound> Rounds => _rounds;
    
    private TurnGamePlayers Players => new(PlayerOneId, PlayerTwoId);

    private ITurnGameRules Rules => TurnGameRulesCatalog.Get(
        GameType ?? throw new InvalidOperationException("The game type has not been selected."));
    
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
            Purpose = TurnGamePurpose.Casual,
            Status = TurnGameStatus.Ready,
            CreatedAtUtc = DateTime.UtcNow
        };
        
        turnGame.Raise(new TurnGameCreatedDomainEvent(turnGame.Id));

        return turnGame;
    }

    public static Result<TurnGame> Schedule(Couple couple)
    {
        ArgumentNullException.ThrowIfNull(couple);

        if (!couple.IsActive)
        {
            return Result.Failure<TurnGame>(TurnGameErrors.CoupleMustBeActive);
        }

        var turnGame = new TurnGame
        {
            Id = Guid.NewGuid(),
            CoupleId = couple.Id,
            PlayerOneId = couple.PartnerOneId,
            PlayerTwoId = couple.PartnerTwoId,
            GameType = null,
            Purpose = TurnGamePurpose.PlanningDecision,
            Status = TurnGameStatus.AwaitingSelection,
            CreatedAtUtc = DateTime.UtcNow
        };

        turnGame.Raise(new TurnGameCreatedDomainEvent(turnGame.Id));
        return turnGame;
    }

    public Result SelectGameType(TurnGameType gameType)
    {
        if (Purpose != TurnGamePurpose.PlanningDecision || Status != TurnGameStatus.AwaitingSelection)
        {
            return Result.Failure(
                GameType.HasValue
                    ? TurnGameErrors.GameTypeAlreadySelected
                    : TurnGameErrors.OnlyScheduledGamesCanBeSelected);
        }

        if (!TurnGameRulesCatalog.TryGetCurrent(gameType, out _))
        {
            return Result.Failure(TurnGameErrors.UnsupportedGameType);
        }

        GameType = gameType;
        Status = TurnGameStatus.Ready;
        return Result.Success();
    }

    public Result StartRound()
    {
        if (Status == TurnGameStatus.AwaitingSelection || GameType is null)
        {
            return Result.Failure(TurnGameErrors.GameTypeMustBeSelected);
        }

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
    
    public Result Play(Guid playerId, ITurnGameMove move) =>
        Play(playerId, move, DateTime.UtcNow);

    public Result Play(Guid playerId, ITurnGameMove move, DateTime playedAtUtc)
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

        if (UsesTimedRounds && playedAtUtc >= activeRound.StartedAtUtc.Add(TimedRoundDuration))
        {
            return Result.Failure(TurnGameErrors.RoundMoveTimeExpired);
        }

        Result playResult = activeRound.Play(
            playerId,
            move,
            Players,
            Rules,
            playedAtUtc);

        if (playResult.IsFailure)
        {
            return playResult;
        }

        if (activeRound.Status == TurnGameRoundStatus.Finished)
        {
            ResolveGame(playedAtUtc);
        }

        return Result.Success();
    }

    public Result ExpireRound(Guid roundId, DateTime expiredAtUtc)
    {
        TurnGameRound? round = _rounds.SingleOrDefault(x => x.Id == roundId);

        if (round is null)
        {
            return Result.Failure(TurnGameErrors.RoundNotFound(roundId));
        }

        if (round.Status == TurnGameRoundStatus.Finished)
        {
            return Result.Success();
        }

        if (!UsesTimedRounds)
        {
            return Result.Failure(TurnGameErrors.RoundDoesNotUseTimer);
        }

        if (Status != TurnGameStatus.InProgress)
        {
            return Result.Failure(TurnGameErrors.GameMustBeInProgress);
        }

        if (expiredAtUtc < round.StartedAtUtc.Add(TimedRoundDuration))
        {
            return Result.Failure(TurnGameErrors.RoundTimerStillRunning);
        }

        round.Expire(Players, expiredAtUtc);
        ResolveGame(expiredAtUtc);

        return Result.Success();
    }

    private bool UsesTimedRounds => GameType is
        TurnGameType.RockPaperScissors or
        TurnGameType.HigherNumberWins;

    private void ResolveGame(DateTime finishedAtUtc)
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
        FinishedAtUtc = finishedAtUtc;

        Raise(new TurnGameFinishedDomainEvent(Id, CoupleId, winnerId, Purpose));
    }
}

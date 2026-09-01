using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.TurnGames;

public class TurnGame : Entity
{
    public Guid Id { get; private set; }

    public Guid CoupleId { get; private set; }

    public Guid? WinnerId { get; private set; }

    public TurnGameType GameType { get; private set; }

    public List<TurnGameRound> Rounds { get; private set; } = [];

    private TurnGame()
    {
    }

    public static TurnGame Create(Guid coupleId, TurnGameType gameType)
    {
        TurnGame turnGame = new TurnGame
        {
            Id = Guid.NewGuid(),
            CoupleId = coupleId,
            GameType = gameType
        };
        
        turnGame.Raise(new TurnGameStartedDomainEvent(turnGame.Id));

        return turnGame;
    }

    public void FinishGame(Guid winnerId)
    {
        WinnerId = winnerId;

        Raise(new TurnGameFinishedDomainEvent(Id));
    }

    public void StartNewRound()
    {
        Rounds.Add(TurnGameRound.Create(Id));
        Raise(new TurnGameRoundStartedDomainEvent(Id));
    }
}

public class TurnGameRound
{
    public Guid Id { get; private set; }
    
    public Guid TurnGameId { get; private set; }

    private TurnGameRound()
    {
    }

    public static TurnGameRound Create(Guid turnGameId)
    {
        return new TurnGameRound
        {
            Id = Guid.NewGuid(),
            TurnGameId = turnGameId
        };
    }
};
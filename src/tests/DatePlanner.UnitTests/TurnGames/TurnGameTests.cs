using DatePlanner.UnitTests.Abstractions;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.TurnGames.Events;
using DatePlanner.Domain.TurnGames.Rounds;
using DatePlanner.Domain.TurnGames.Rules.HigherNumberWins;
using DatePlanner.Domain.TurnGames.Rules.RockPaperScissors;
using FluentAssertions;

namespace DatePlanner.UnitTests.TurnGames;

public class TurnGameTests : BaseTest
{
    [Theory]
    [InlineData(TurnGameType.RockPaperScissors)]
    [InlineData(TurnGameType.HigherNumberWins)]
    public void Create_ShouldCreateReadyGame_FromActiveCouple(TurnGameType gameType)
    {
        Couple couple = CreateCouple();

        Result<TurnGame> result = TurnGame.Create(couple, gameType);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.CoupleId.Should().Be(couple.Id);
        result.Value.PlayerOneId.Should().Be(couple.PartnerOneId);
        result.Value.PlayerTwoId.Should().Be(couple.PartnerTwoId);
        result.Value.GameType.Should().Be(gameType);
        result.Value.Status.Should().Be(TurnGameStatus.Ready);
        result.Value.Rounds.Should().BeEmpty();
        result.Value.WinnerId.Should().BeNull();
        result.Value.CreatedAtUtc.Should().NotBe(default);
    }

    [Fact]
    public void Create_ShouldPublishCreatedEvent_WithGameId()
    {
        TurnGame game = CreateGame(TurnGameType.RockPaperScissors);

        TurnGameCreatedDomainEvent domainEvent =
            AssertDomainEventWasPublished<TurnGameCreatedDomainEvent>(game);

        domainEvent.TurnGameId.Should().Be(game.Id);
        domainEvent.Id.Should().NotBeEmpty();
        domainEvent.OccurredOnUtc.Should().NotBe(default);
        game.DomainEvents.Should().ContainSingle();
    }

    [Fact]
    public void Create_ShouldRejectUnsupportedGameType()
    {
        Result<TurnGame> result = TurnGame.Create(CreateCouple(), (TurnGameType)999);

        result.IsFailure.Should().BeTrue();
        result.Error.ShouldHaveCode("TurnGame.UnsupportedGameType", ErrorType.Problem);
    }

    [Fact]
    public void Create_ShouldThrow_WhenCoupleIsNull()
    {
        Action act = () => TurnGame.Create(null!, TurnGameType.RockPaperScissors);

        act.Should().Throw<ArgumentNullException>().WithParameterName("couple");
    }

    [Fact]
    public void StartRound_ShouldStartGameAndFirstRound_AndPublishBothEvents()
    {
        TurnGame game = CreateGame(TurnGameType.RockPaperScissors);

        Result result = game.StartRound();

        result.IsSuccess.Should().BeTrue();
        game.Status.Should().Be(TurnGameStatus.InProgress);
        game.StartedAtUtc.Should().NotBeNull();
        game.StartedAtUtc!.Value.Should().NotBe(default);
        TurnGameRound round = game.Rounds.Should().ContainSingle().Subject;
        round.TurnGameId.Should().Be(game.Id);
        round.Number.Should().Be(1);
        round.Status.Should().Be(TurnGameRoundStatus.InProgress);
        round.IsDraw.Should().BeFalse();
        round.Moves.Should().BeEmpty();

        TurnGameStartedDomainEvent started = AssertDomainEventWasPublished<TurnGameStartedDomainEvent>(game);
        started.TurnGameId.Should().Be(game.Id);
        TurnGameRoundStartedDomainEvent roundStarted =
            AssertDomainEventWasPublished<TurnGameRoundStartedDomainEvent>(game);
        roundStarted.TurnGameId.Should().Be(game.Id);
        roundStarted.RoundId.Should().Be(round.Id);
        roundStarted.RoundNumber.Should().Be(1);
        game.DomainEvents.Should().HaveCount(3);
    }

    [Fact]
    public void StartRound_ShouldRejectSecondActiveRound_WithoutChangingStateOrEvents()
    {
        TurnGame game = CreateStartedGame(TurnGameType.RockPaperScissors);
        int eventCount = game.DomainEvents.Count;

        Result result = game.StartRound();

        result.Error.ShouldHaveCode("TurnGame.ActiveRoundExists", ErrorType.Conflict);
        game.Rounds.Should().ContainSingle();
        game.DomainEvents.Should().HaveCount(eventCount);
    }

    [Fact]
    public void Play_ShouldRejectMove_WhenGameHasNotStarted()
    {
        TurnGame game = CreateGame(TurnGameType.RockPaperScissors);

        Result result = game.Play(game.PlayerOneId, RockPaperScissorsMove.Rock);

        result.Error.ShouldHaveCode("TurnGame.NotInProgress", ErrorType.Conflict);
        game.Rounds.Should().BeEmpty();
        game.Status.Should().Be(TurnGameStatus.Ready);
    }

    [Fact]
    public void Play_ShouldRejectNonParticipant_WithoutAddingMove()
    {
        TurnGame game = CreateStartedGame(TurnGameType.RockPaperScissors);

        Result result = game.Play(Guid.NewGuid(), RockPaperScissorsMove.Rock);

        result.Error.ShouldHaveCode("TurnGame.NotParticipant", ErrorType.Problem);
        game.Rounds.Single().Moves.Should().BeEmpty();
    }

    [Fact]
    public void Play_ShouldRejectMoveFromDifferentGame_WithoutAddingMove()
    {
        TurnGame game = CreateStartedGame(TurnGameType.RockPaperScissors);

        Result result = game.Play(game.PlayerOneId, HigherNumberMove.Six);

        result.Error.ShouldHaveCode("TurnGame.IncompatibleMove", ErrorType.Problem);
        game.Rounds.Single().Moves.Should().BeEmpty();
    }

    [Fact]
    public void Play_ShouldRecordMove_WithPlayerCodeRoundAndSequence()
    {
        TurnGame game = CreateStartedGame(TurnGameType.RockPaperScissors);
        TurnGameRound round = game.Rounds.Single();

        Result result = game.Play(game.PlayerOneId, RockPaperScissorsMove.Paper);

        result.IsSuccess.Should().BeTrue();
        TurnGameMove move = round.Moves.Should().ContainSingle().Subject;
        move.Id.Should().NotBeEmpty();
        move.TurnGameRoundId.Should().Be(round.Id);
        move.PlayerId.Should().Be(game.PlayerOneId);
        move.Sequence.Should().Be(1);
        move.Code.Should().Be("paper");
        move.PlayedAtUtc.Should().NotBe(default);
        round.Status.Should().Be(TurnGameRoundStatus.InProgress);
    }

    [Fact]
    public void Play_ShouldRejectSecondMoveBySamePlayer_WithoutAddingAnotherMove()
    {
        TurnGame game = CreateStartedGame(TurnGameType.RockPaperScissors);
        game.Play(game.PlayerOneId, RockPaperScissorsMove.Rock);

        Result result = game.Play(game.PlayerOneId, RockPaperScissorsMove.Paper);

        result.Error.ShouldHaveCode("TurnGame.PlayerAlreadyMoved", ErrorType.Conflict);
        game.Rounds.Single().Moves.Should().ContainSingle().Which.Code.Should().Be("rock");
    }

    [Fact]
    public void Play_ShouldFinishRoundAsDraw_WhenMovesMatch()
    {
        TurnGame game = CreateStartedGame(TurnGameType.RockPaperScissors);
        game.Play(game.PlayerOneId, RockPaperScissorsMove.Rock);

        Result result = game.Play(game.PlayerTwoId, RockPaperScissorsMove.Rock);

        result.IsSuccess.Should().BeTrue();
        TurnGameRound round = game.Rounds.Single();
        round.Status.Should().Be(TurnGameRoundStatus.Finished);
        round.Moves.Select(x => x.Sequence).Should().Equal(1, 2);
        round.WinnerId.Should().BeNull();
        round.IsDraw.Should().BeTrue();
        round.FinishedAtUtc.Should().NotBeNull();
        game.Status.Should().Be(TurnGameStatus.InProgress);
        game.WinnerId.Should().BeNull();
    }

    [Fact]
    public void StartRound_ShouldStartNextNumberedRound_AfterPreviousRoundFinishes()
    {
        TurnGame game = CreateStartedGame(TurnGameType.HigherNumberWins);
        CompleteHigherNumberRound(game, HigherNumberMove.One, HigherNumberMove.One);

        Result result = game.StartRound();

        result.IsSuccess.Should().BeTrue();
        game.Rounds.Select(x => x.Number).Should().Equal(1, 2);
        game.Rounds.Last().Status.Should().Be(TurnGameRoundStatus.InProgress);
        game.DomainEvents.OfType<TurnGameStartedDomainEvent>().Should().ContainSingle();
        game.DomainEvents.OfType<TurnGameRoundStartedDomainEvent>().Should().HaveCount(2);
        game.DomainEvents.OfType<TurnGameRoundStartedDomainEvent>().Last().RoundNumber.Should().Be(2);
    }

    [Fact]
    public void Play_ShouldRejectMove_WhenPreviousRoundFinishedAndNextRoundHasNotStarted()
    {
        TurnGame game = CreateStartedGame(TurnGameType.HigherNumberWins);
        CompleteHigherNumberRound(game, HigherNumberMove.One, HigherNumberMove.One);
        int existingMoves = game.Rounds.Single().Moves.Count;

        Result result = game.Play(game.PlayerOneId, HigherNumberMove.Six);

        result.Error.ShouldHaveCode("TurnGame.NoActiveRound", ErrorType.Conflict);
        game.Status.Should().Be(TurnGameStatus.InProgress);
        game.Rounds.Should().ContainSingle();
        game.Rounds.Single().Moves.Should().HaveCount(existingMoves);
    }

    [Fact]
    public void Play_ShouldFinishGameAndPublishWinner_AfterPlayerOneWinsTwoRounds()
    {
        TurnGame game = CreateStartedGame(TurnGameType.RockPaperScissors);
        CompleteRockPaperScissorsRound(game, RockPaperScissorsMove.Rock, RockPaperScissorsMove.Scissors);
        game.Status.Should().Be(TurnGameStatus.InProgress);
        game.StartRound();

        CompleteRockPaperScissorsRound(game, RockPaperScissorsMove.Paper, RockPaperScissorsMove.Rock);

        game.Status.Should().Be(TurnGameStatus.Finished);
        game.WinnerId.Should().Be(game.PlayerOneId);
        game.FinishedAtUtc.Should().NotBeNull();
        game.FinishedAtUtc!.Value.Should().NotBe(default);
        TurnGameFinishedDomainEvent finished =
            AssertDomainEventWasPublished<TurnGameFinishedDomainEvent>(game);
        finished.TurnGameId.Should().Be(game.Id);
        finished.CoupleId.Should().Be(game.CoupleId);
        finished.WinnerId.Should().Be(game.PlayerOneId);
    }

    [Fact]
    public void Play_ShouldSelectPlayerTwoAsWinner_AfterTwoHigherNumberWins()
    {
        TurnGame game = CreateStartedGame(TurnGameType.HigherNumberWins);
        CompleteHigherNumberRound(game, HigherNumberMove.One, HigherNumberMove.Six);
        game.StartRound();

        CompleteHigherNumberRound(game, HigherNumberMove.Two, HigherNumberMove.Three);

        game.Status.Should().Be(TurnGameStatus.Finished);
        game.WinnerId.Should().Be(game.PlayerTwoId);
        game.DomainEvents.OfType<TurnGameFinishedDomainEvent>()
            .Should().ContainSingle().Which.WinnerId.Should().Be(game.PlayerTwoId);
    }

    [Fact]
    public void Game_ShouldContinueAfterDraw_UntilAPlayerWinsTwoRounds()
    {
        TurnGame game = CreateStartedGame(TurnGameType.HigherNumberWins);
        CompleteHigherNumberRound(game, HigherNumberMove.Three, HigherNumberMove.Three);
        game.StartRound();
        CompleteHigherNumberRound(game, HigherNumberMove.Six, HigherNumberMove.One);

        game.Status.Should().Be(TurnGameStatus.InProgress);
        game.WinnerId.Should().BeNull();
        game.DomainEvents.OfType<TurnGameFinishedDomainEvent>().Should().BeEmpty();
    }

    [Fact]
    public void FinishedGame_ShouldRejectStartingRoundAndPlaying_WithoutChangingState()
    {
        TurnGame game = CreateFinishedGame();
        int rounds = game.Rounds.Count;
        int events = game.DomainEvents.Count;

        Result startResult = game.StartRound();
        Result playResult = game.Play(game.PlayerOneId, RockPaperScissorsMove.Rock);

        startResult.Error.ShouldHaveCode("TurnGame.AlreadyFinished", ErrorType.Conflict);
        playResult.Error.ShouldHaveCode("TurnGame.NotInProgress", ErrorType.Conflict);
        game.Rounds.Should().HaveCount(rounds);
        game.DomainEvents.Should().HaveCount(events);
        game.Status.Should().Be(TurnGameStatus.Finished);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemovePreviouslyPublishedEvents()
    {
        TurnGame game = CreateStartedGame(TurnGameType.RockPaperScissors);
        game.DomainEvents.Should().NotBeEmpty();

        game.ClearDomainEvents();

        game.DomainEvents.Should().BeEmpty();
    }

    private static Couple CreateCouple() => Couple.Create(Guid.NewGuid(), Guid.NewGuid()).Value;

    private static TurnGame CreateGame(TurnGameType gameType) => TurnGame.Create(CreateCouple(), gameType).Value;

    private static TurnGame CreateStartedGame(TurnGameType gameType)
    {
        TurnGame game = CreateGame(gameType);
        game.StartRound();
        return game;
    }

    private static TurnGame CreateFinishedGame()
    {
        TurnGame game = CreateStartedGame(TurnGameType.RockPaperScissors);
        CompleteRockPaperScissorsRound(game, RockPaperScissorsMove.Rock, RockPaperScissorsMove.Scissors);
        game.StartRound();
        CompleteRockPaperScissorsRound(game, RockPaperScissorsMove.Paper, RockPaperScissorsMove.Rock);
        return game;
    }

    private static void CompleteRockPaperScissorsRound(
        TurnGame game,
        RockPaperScissorsMove playerOneMove,
        RockPaperScissorsMove playerTwoMove)
    {
        game.Play(game.PlayerOneId, playerOneMove);
        game.Play(game.PlayerTwoId, playerTwoMove);
    }

    private static void CompleteHigherNumberRound(
        TurnGame game,
        HigherNumberMove playerOneMove,
        HigherNumberMove playerTwoMove)
    {
        game.Play(game.PlayerOneId, playerOneMove);
        game.Play(game.PlayerTwoId, playerTwoMove);
    }
}

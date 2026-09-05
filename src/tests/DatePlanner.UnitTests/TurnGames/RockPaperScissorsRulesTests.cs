using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.TurnGames.Rules;
using DatePlanner.Domain.TurnGames.Rules.RockPaperScissors;
using FluentAssertions;
using DatePlanner.UnitTests.Abstractions;

namespace DatePlanner.UnitTests.TurnGames;

public class RockPaperScissorsRulesTests
{
    private readonly RockPaperScissorsRules _rules = new();

    [Theory]
    [InlineData("rock")]
    [InlineData("paper")]
    [InlineData("scissors")]
    public void ValidateNextMove_ShouldAcceptEachSupportedMove(string moveCode)
    {
        Result result = _rules.ValidateNextMove(PlayerPosition.PlayerOne, moveCode, []);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("Rock")]
    [InlineData("lizard")]
    public void ValidateNextMove_ShouldRejectUnsupportedMove(string moveCode)
    {
        Result result = _rules.ValidateNextMove(PlayerPosition.PlayerOne, moveCode, []);

        result.Error.ShouldHaveCode("TurnGame.InvalidMove", ErrorType.Problem);
    }

    [Fact]
    public void ValidateNextMove_ShouldRejectPlayerWhoAlreadyMoved()
    {
        MoveView existing = new(PlayerPosition.PlayerOne, "rock", 1);

        Result result = _rules.ValidateNextMove(PlayerPosition.PlayerOne, "paper", [existing]);

        result.Error.ShouldHaveCode("TurnGame.PlayerAlreadyMoved", ErrorType.Conflict);
    }

    [Fact]
    public void EvaluateRound_ShouldRemainPending_UntilBothPlayersMove()
    {
        RoundDecision empty = _rules.EvaluateRound([]);
        RoundDecision oneMove = _rules.EvaluateRound([new MoveView(PlayerPosition.PlayerOne, "rock", 1)]);

        empty.Kind.Should().Be(RoundDecisionKind.Pending);
        empty.Winner.Should().BeNull();
        oneMove.Kind.Should().Be(RoundDecisionKind.Pending);
        oneMove.Winner.Should().BeNull();
    }

    [Theory]
    [InlineData("rock", "scissors", PlayerPosition.PlayerOne)]
    [InlineData("scissors", "paper", PlayerPosition.PlayerOne)]
    [InlineData("paper", "rock", PlayerPosition.PlayerOne)]
    [InlineData("scissors", "rock", PlayerPosition.PlayerTwo)]
    [InlineData("paper", "scissors", PlayerPosition.PlayerTwo)]
    [InlineData("rock", "paper", PlayerPosition.PlayerTwo)]
    public void EvaluateRound_ShouldSelectWinner_ForEveryNonDrawMatchup(
        string playerOneMove,
        string playerTwoMove,
        PlayerPosition expectedWinner)
    {
        MoveView[] moves =
        [
            new(PlayerPosition.PlayerOne, playerOneMove, 1),
            new(PlayerPosition.PlayerTwo, playerTwoMove, 2)
        ];

        RoundDecision decision = _rules.EvaluateRound(moves);

        decision.Kind.Should().Be(RoundDecisionKind.Winner);
        decision.Winner.Should().Be(expectedWinner);
    }

    [Theory]
    [InlineData("rock")]
    [InlineData("paper")]
    [InlineData("scissors")]
    public void EvaluateRound_ShouldReturnDraw_WhenMovesMatch(string moveCode)
    {
        MoveView[] moves =
        [
            new(PlayerPosition.PlayerOne, moveCode, 1),
            new(PlayerPosition.PlayerTwo, moveCode, 2)
        ];

        RoundDecision decision = _rules.EvaluateRound(moves);

        decision.Kind.Should().Be(RoundDecisionKind.Draw);
        decision.Winner.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void EvaluateGame_ShouldContinue_WhenPlayerHasFewerThanTwoWins(int wins)
    {
        RoundView[] rounds = Enumerable.Repeat(
            new RoundView(RoundDecisionKind.Winner, PlayerPosition.PlayerOne), wins).ToArray();

        GameDecision decision = _rules.EvaluateGame(rounds);

        decision.Should().BeOfType<GameDecision.ContinuePlaying>();
    }

    [Theory]
    [InlineData(PlayerPosition.PlayerOne)]
    [InlineData(PlayerPosition.PlayerTwo)]
    public void EvaluateGame_ShouldDeclarePlayer_WhenPlayerReachesTwoWins(PlayerPosition player)
    {
        RoundView[] rounds =
        [
            new(RoundDecisionKind.Winner, player),
            new(RoundDecisionKind.Draw, null),
            new(RoundDecisionKind.Winner, player)
        ];

        GameDecision decision = _rules.EvaluateGame(rounds);

        decision.Should().BeOfType<GameDecision.Won>()
            .Which.Winner.Should().Be(player);
    }

    [Fact]
    public void RockPaperScissorsMoves_ShouldExposeTheThreeSupportedCodes()
    {
        RockPaperScissorsMove[] moves =
        [
            RockPaperScissorsMove.Rock,
            RockPaperScissorsMove.Paper,
            RockPaperScissorsMove.Scissors
        ];

        moves.Select(x => x.Code).Should().Equal("rock", "paper", "scissors");
        moves.Should().OnlyContain(x => x.GameType == TurnGameType.RockPaperScissors);
    }
}

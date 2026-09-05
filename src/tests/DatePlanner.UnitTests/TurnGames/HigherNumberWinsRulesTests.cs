using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.TurnGames.Rules;
using DatePlanner.Domain.TurnGames.Rules.HigherNumberWins;
using FluentAssertions;
using DatePlanner.UnitTests.Abstractions;

namespace DatePlanner.UnitTests.TurnGames;

public class HigherNumberWinsRulesTests
{
    private readonly HigherNumberWinsRules _rules = new();

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("5")]
    [InlineData("6")]
    public void ValidateNextMove_ShouldAcceptNumber_AtAndWithinBoundaries(string moveCode)
    {
        Result result = _rules.ValidateNextMove(PlayerPosition.PlayerOne, moveCode, []);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("0")]
    [InlineData("7")]
    [InlineData("not-a-number")]
    [InlineData("")]
    public void ValidateNextMove_ShouldRejectNumber_OutsideOneThroughSix(string moveCode)
    {
        Result result = _rules.ValidateNextMove(PlayerPosition.PlayerOne, moveCode, []);

        result.Error.ShouldHaveCode("TurnGame.InvalidMove", ErrorType.Problem);
    }

    [Fact]
    public void ValidateNextMove_ShouldRejectPlayerWhoAlreadyMoved()
    {
        MoveView existing = new(PlayerPosition.PlayerTwo, "1", 1);

        Result result = _rules.ValidateNextMove(PlayerPosition.PlayerTwo, "6", [existing]);

        result.Error.ShouldHaveCode("TurnGame.PlayerAlreadyMoved", ErrorType.Conflict);
    }

    [Fact]
    public void EvaluateRound_ShouldRemainPending_UntilBothPlayersMove()
    {
        RoundDecision decision = _rules.EvaluateRound(
            [new MoveView(PlayerPosition.PlayerOne, "6", 1)]);

        decision.Kind.Should().Be(RoundDecisionKind.Pending);
        decision.Winner.Should().BeNull();
    }

    [Theory]
    [InlineData("6", "1", PlayerPosition.PlayerOne)]
    [InlineData("1", "6", PlayerPosition.PlayerTwo)]
    [InlineData("4", "3", PlayerPosition.PlayerOne)]
    [InlineData("3", "4", PlayerPosition.PlayerTwo)]
    public void EvaluateRound_ShouldSelectPlayerWithHigherNumber(
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
    [InlineData("1")]
    [InlineData("6")]
    public void EvaluateRound_ShouldReturnDraw_WhenNumbersMatch(string moveCode)
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
            new RoundView(RoundDecisionKind.Winner, PlayerPosition.PlayerTwo), wins).ToArray();

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
    public void HigherNumberMoves_ShouldExposeCodesForAllSixAllowedValues()
    {
        HigherNumberMove[] moves =
        [
            HigherNumberMove.One,
            HigherNumberMove.Two,
            HigherNumberMove.Three,
            HigherNumberMove.Four,
            HigherNumberMove.Five,
            HigherNumberMove.Six
        ];

        moves.Select(x => x.Code).Should().Equal("1", "2", "3", "4", "5", "6");
        moves.Should().OnlyContain(x => x.GameType == TurnGameType.HigherNumberWins);
    }
}

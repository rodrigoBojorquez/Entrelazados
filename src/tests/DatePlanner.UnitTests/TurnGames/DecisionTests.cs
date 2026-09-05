using DatePlanner.Domain.TurnGames.Rules;
using FluentAssertions;

namespace DatePlanner.UnitTests.TurnGames;

public class DecisionTests
{
    [Theory]
    [InlineData(PlayerPosition.PlayerOne)]
    [InlineData(PlayerPosition.PlayerTwo)]
    public void RoundDecisionWonBy_ShouldCarryWinner(PlayerPosition winner)
    {
        RoundDecision decision = RoundDecision.WonBy(winner);

        decision.Kind.Should().Be(RoundDecisionKind.Winner);
        decision.Winner.Should().Be(winner);
    }

    [Fact]
    public void RoundDecisionWonBy_ShouldRejectUndefinedPlayerPosition()
    {
        Action act = () => RoundDecision.WonBy((PlayerPosition)999);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("player");
    }

    [Fact]
    public void GameDecisionWonBy_ShouldRejectUndefinedPlayerPosition()
    {
        Action act = () => GameDecision.WonBy((PlayerPosition)999);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("winner");
    }
}

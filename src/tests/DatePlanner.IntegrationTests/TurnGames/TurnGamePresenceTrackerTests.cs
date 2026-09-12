using DatePlanner.Api.Endpoints.TurnGames;
using FluentAssertions;

namespace DatePlanner.IntegrationTests.TurnGames;

public sealed class TurnGamePresenceTrackerTests
{
    [Fact]
    public void Join_ShouldReportReadyOnlyWhenSecondDistinctPlayerConnects()
    {
        var tracker = new TurnGamePresenceTracker();
        Guid gameId = Guid.NewGuid();
        Guid playerOneId = Guid.NewGuid();
        Guid playerTwoId = Guid.NewGuid();

        TurnGamePresenceJoinResult firstConnection = tracker.Join(
            gameId,
            playerOneId,
            "player-one-tab-one",
            playerOneId,
            playerTwoId);
        TurnGamePresenceJoinResult duplicateConnection = tracker.Join(
            gameId,
            playerOneId,
            "player-one-tab-two",
            playerOneId,
            playerTwoId);
        TurnGamePresenceJoinResult secondPlayerConnection = tracker.Join(
            gameId,
            playerTwoId,
            "player-two-tab-one",
            playerOneId,
            playerTwoId);
        TurnGamePresenceJoinResult secondPlayerDuplicate = tracker.Join(
            gameId,
            playerTwoId,
            "player-two-tab-two",
            playerOneId,
            playerTwoId);

        firstConnection.Presence.BothPlayersConnected.Should().BeFalse();
        duplicateConnection.BecameFullyConnected.Should().BeFalse();
        secondPlayerConnection.Presence.BothPlayersConnected.Should().BeTrue();
        secondPlayerConnection.BecameFullyConnected.Should().BeTrue();
        secondPlayerDuplicate.BecameFullyConnected.Should().BeFalse();
    }
}

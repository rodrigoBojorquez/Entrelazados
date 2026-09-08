using DatePlanner.Application.TurnGames.CreateTurnGame;
using DatePlanner.Application.TurnGames.GetTurnGame;
using DatePlanner.Application.TurnGames.PlayTurnGameMove;
using DatePlanner.Application.TurnGames.StartTurnGameRound;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.TurnGames.Rounds;
using DatePlanner.IntegrationTests.Abstractions;
using FluentAssertions;

namespace DatePlanner.IntegrationTests.TurnGames;

public sealed class TurnGameTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task TurnGame_ShouldCompleteBestOfThreeWorkflow()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        Result<Guid> creationResult = await Sender.Send(new CreateTurnGameCommand(
            couple.PartnerOne.Id,
            TurnGameType.RockPaperScissors));

        creationResult.IsSuccess.Should().BeTrue();

        await PlayRoundAsync(
            couple,
            creationResult.Value,
            playerOneMove: "rock",
            playerTwoMove: "scissors");
        await PlayRoundAsync(
            couple,
            creationResult.Value,
            playerOneMove: "paper",
            playerTwoMove: "rock");

        Result<TurnGameResponse> queryResult = await Sender.Send(
            new GetTurnGameQuery(couple.PartnerTwo.Id, creationResult.Value));

        queryResult.IsSuccess.Should().BeTrue();
        queryResult.Value.GameType.Should().Be(TurnGameType.RockPaperScissors);
        queryResult.Value.Status.Should().Be(TurnGameStatus.Finished);
        queryResult.Value.WinnerId.Should().Be(couple.PartnerOne.Id);
        queryResult.Value.Rounds.Should().HaveCount(2);
        queryResult.Value.Rounds.Should().OnlyContain(
            round => round.Status == TurnGameRoundStatus.Finished);
        queryResult.Value.Rounds.SelectMany(round => round.Moves)
            .Should().HaveCount(4)
            .And.OnlyContain(move => move.Code != null);
    }

    [Fact]
    public async Task GetTurnGame_ShouldHideOpponentMoveWhileRoundIsInProgress()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        Result<Guid> creationResult = await Sender.Send(new CreateTurnGameCommand(
            couple.PartnerOne.Id,
            TurnGameType.HigherNumberWins));
        await Sender.Send(new StartTurnGameRoundCommand(
            couple.PartnerOne.Id,
            creationResult.Value));
        await Sender.Send(new PlayTurnGameMoveCommand(
            couple.PartnerOne.Id,
            creationResult.Value,
            "6"));

        Result<TurnGameResponse> ownerView = await Sender.Send(
            new GetTurnGameQuery(couple.PartnerOne.Id, creationResult.Value));
        Result<TurnGameResponse> opponentView = await Sender.Send(
            new GetTurnGameQuery(couple.PartnerTwo.Id, creationResult.Value));

        ownerView.Value.Rounds.Single().Moves.Single().Code.Should().Be("6");
        opponentView.Value.Rounds.Single().Moves.Single().Code.Should().BeNull();
    }

    [Fact]
    public async Task PlayTurnGameMove_ShouldFail_WhenMoveCodeIsInvalid()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        Result<Guid> creationResult = await Sender.Send(new CreateTurnGameCommand(
            couple.PartnerOne.Id,
            TurnGameType.HigherNumberWins));
        await Sender.Send(new StartTurnGameRoundCommand(
            couple.PartnerOne.Id,
            creationResult.Value));

        Result result = await Sender.Send(new PlayTurnGameMoveCommand(
            couple.PartnerOne.Id,
            creationResult.Value,
            "7"));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("TurnGame.InvalidMove");
    }

    private async Task PlayRoundAsync(
        RegisteredCouple couple,
        Guid turnGameId,
        string playerOneMove,
        string playerTwoMove)
    {
        Result startResult = await Sender.Send(
            new StartTurnGameRoundCommand(couple.PartnerOne.Id, turnGameId));
        Result playerOneResult = await Sender.Send(
            new PlayTurnGameMoveCommand(couple.PartnerOne.Id, turnGameId, playerOneMove));
        Result playerTwoResult = await Sender.Send(
            new PlayTurnGameMoveCommand(couple.PartnerTwo.Id, turnGameId, playerTwoMove));

        startResult.IsSuccess.Should().BeTrue();
        playerOneResult.IsSuccess.Should().BeTrue();
        playerTwoResult.IsSuccess.Should().BeTrue();
    }
}

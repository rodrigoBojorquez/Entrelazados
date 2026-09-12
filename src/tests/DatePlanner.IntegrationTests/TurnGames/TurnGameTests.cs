using System.Text.Json.Serialization;
using DatePlanner.Api.Endpoints.TurnGames;
using DatePlanner.Application.Authentication;
using DatePlanner.Application.Authentication.Login;
using DatePlanner.Application.TurnGames.CreateTurnGame;
using DatePlanner.Application.TurnGames.GetTurnGame;
using DatePlanner.Application.TurnGames.GetCurrentScheduledTurnGame;
using DatePlanner.Application.TurnGames.PlayTurnGameMove;
using DatePlanner.Application.TurnGames.ListTurnGames;
using DatePlanner.Application.TurnGames.SelectTurnGameType;
using DatePlanner.Application.TurnGames.StartTurnGameRound;
using DatePlanner.Application.Dates.AddItineraryItem;
using DatePlanner.Application.Dates.AttendDatePlan;
using DatePlanner.Application.Dates.ScheduleDatePlan;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Dates;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Domain.TurnGames.Rounds;
using DatePlanner.Infrastructure.Database;
using DatePlanner.IntegrationTests.Abstractions;
using FluentAssertions;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DatePlanner.IntegrationTests.TurnGames;

public sealed class TurnGameTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    private readonly IntegrationTestWebAppFactory _factory = factory;

    [Fact]
    public async Task CasualTurnGame_ShouldCompleteWithoutCreatingDatePlan()
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
        queryResult.Value.Purpose.Should().Be(TurnGamePurpose.Casual);
        queryResult.Value.Status.Should().Be(TurnGameStatus.Finished);
        queryResult.Value.WinnerId.Should().Be(couple.PartnerOne.Id);
        queryResult.Value.Rounds.Should().HaveCount(2);
        queryResult.Value.Rounds.Should().OnlyContain(
            round => round.Status == TurnGameRoundStatus.Finished);
        queryResult.Value.Rounds.SelectMany(round => round.Moves)
            .Should().HaveCount(4)
            .And.OnlyContain(move => move.Code != null);

        DatePlannerDbContext dbContext = Services.GetRequiredService<DatePlannerDbContext>();
        (await dbContext.DatePlans
            .Where(plan => plan.CoupleId == couple.Id)
            .ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task PlanningCycle_ShouldCreateDateForWinner_ThenScheduleNextGameAfterAttendance()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        DatePlannerDbContext dbContext = Services.GetRequiredService<DatePlannerDbContext>();
        TurnGame scheduledGame = await dbContext.TurnGames.SingleAsync(game =>
            game.CoupleId == couple.Id &&
            game.Purpose == TurnGamePurpose.PlanningDecision &&
            game.Status == TurnGameStatus.AwaitingSelection);

        Result firstSelection = await Sender.Send(new SelectTurnGameTypeCommand(
            couple.PartnerTwo.Id,
            scheduledGame.Id,
            TurnGameType.HigherNumberWins));
        Result secondSelection = await Sender.Send(new SelectTurnGameTypeCommand(
            couple.PartnerOne.Id,
            scheduledGame.Id,
            TurnGameType.RockPaperScissors));

        firstSelection.IsSuccess.Should().BeTrue();
        secondSelection.IsFailure.Should().BeTrue();
        secondSelection.Error.Code.Should().Be("TurnGame.GameTypeAlreadySelected");

        await PlayRoundAsync(couple, scheduledGame.Id, "1", "6");
        await PlayRoundAsync(couple, scheduledGame.Id, "2", "5");

        DatePlan datePlan = await dbContext.DatePlans
            .SingleAsync(plan => plan.CoupleId == couple.Id);
        datePlan.PartnerWhoPlannedId.Should().Be(couple.PartnerTwo.Id);
        datePlan.IsCollaborative.Should().BeFalse();
        datePlan.Status.Should().Be(DateStatus.Planning);

        await Sender.Send(new AddItineraryItemCommand(
            couple.PartnerTwo.Id,
            datePlan.Id,
            "Cena sorpresa",
            null,
            null,
            new TimeOnly(20, 0)));
        await Sender.Send(new ScheduleDatePlanCommand(
            couple.PartnerTwo.Id,
            datePlan.Id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))));
        Result attendanceResult = await Sender.Send(new AttendDatePlanCommand(
            couple.PartnerOne.Id,
            datePlan.Id));

        attendanceResult.IsSuccess.Should().BeTrue();
        datePlan.Status.Should().Be(DateStatus.Attended);

        TurnGame nextGame = await dbContext.TurnGames.SingleAsync(game =>
            game.CoupleId == couple.Id &&
            game.Purpose == TurnGamePurpose.PlanningDecision &&
            game.Status == TurnGameStatus.AwaitingSelection);
        nextGame.Id.Should().NotBe(scheduledGame.Id);
        nextGame.GameType.Should().BeNull();
        RecordingTurnGameNotifier notifier =
            Services.GetRequiredService<RecordingTurnGameNotifier>();
        notifier.Notifications.Should().Contain(notification =>
            notification.TurnGameId == nextGame.Id &&
            notification.PartnerOneId == couple.PartnerOne.Id &&
            notification.PartnerTwoId == couple.PartnerTwo.Id);

        Result<CurrentScheduledTurnGameResponse> currentGameResult =
            await Sender.Send(new GetCurrentScheduledTurnGameQuery(couple.PartnerOne.Id));
        currentGameResult.Value.Game!.Id.Should().Be(nextGame.Id);

        Result<IReadOnlyCollection<TurnGameSummaryResponse>> historyResult =
            await Sender.Send(new ListTurnGamesQuery(couple.PartnerOne.Id));
        historyResult.IsSuccess.Should().BeTrue();
        historyResult.Value.Should().Contain(game =>
            game.Id == scheduledGame.Id &&
            game.WinnerId == couple.PartnerTwo.Id &&
            game.Purpose == TurnGamePurpose.PlanningDecision);
    }

    [Fact]
    public async Task TurnGameHub_ShouldStartSameGameOnlyAfterBothPlayersJoin()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        DatePlannerDbContext dbContext = Services.GetRequiredService<DatePlannerDbContext>();
        TurnGame scheduledGame = await dbContext.TurnGames.SingleAsync(game =>
            game.CoupleId == couple.Id &&
            game.Purpose == TurnGamePurpose.PlanningDecision &&
            game.Status == TurnGameStatus.AwaitingSelection);
        AuthenticationResponse partnerOneSession = (await Sender.Send(new LoginCommand(
            couple.PartnerOne.Email,
            couple.PartnerOne.Password))).Value;
        AuthenticationResponse partnerTwoSession = (await Sender.Send(new LoginCommand(
            couple.PartnerTwo.Email,
            couple.PartnerTwo.Password))).Value;

        await using HubConnection partnerOneConnection =
            CreateHubConnection(partnerOneSession.AccessToken);
        await using HubConnection partnerTwoConnection =
            CreateHubConnection(partnerTwoSession.AccessToken);
        var partnerOneSelection = new TaskCompletionSource<TurnGameResponse>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var partnerTwoSelection = new TaskCompletionSource<TurnGameResponse>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var partnerOneStarted = new TaskCompletionSource<TurnGameResponse>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        partnerOneConnection.On<TurnGameResponse>("TurnGameSelected", game =>
            partnerOneSelection.TrySetResult(game));
        partnerTwoConnection.On<TurnGameResponse>("TurnGameSelected", game =>
            partnerTwoSelection.TrySetResult(game));
        partnerOneConnection.On<TurnGameResponse>("TurnGameUpdated", game =>
        {
            if (game.Status == TurnGameStatus.InProgress)
            {
                partnerOneStarted.TrySetResult(game);
            }
        });

        await partnerOneConnection.StartAsync();
        await partnerTwoConnection.StartAsync();

        TurnGameHubResponse<TurnGameResponse> selectionResponse =
            await partnerOneConnection.InvokeAsync<TurnGameHubResponse<TurnGameResponse>>(
                "SelectScheduledGame",
                scheduledGame.Id,
                TurnGameType.RockPaperScissors);
        TurnGameResponse firstSelection = await partnerOneSelection.Task.WaitAsync(
            TimeSpan.FromSeconds(5));
        TurnGameResponse secondSelection = await partnerTwoSelection.Task.WaitAsync(
            TimeSpan.FromSeconds(5));

        selectionResponse.IsSuccess.Should().BeTrue();
        firstSelection.Id.Should().Be(scheduledGame.Id);
        secondSelection.Id.Should().Be(scheduledGame.Id);
        firstSelection.GameType.Should().Be(TurnGameType.RockPaperScissors);
        secondSelection.GameType.Should().Be(firstSelection.GameType);

        TurnGameHubResponse<TurnGameResponse> firstJoin =
            await partnerOneConnection.InvokeAsync<TurnGameHubResponse<TurnGameResponse>>(
                "JoinTurnGame",
                scheduledGame.Id);
        firstJoin.Value!.Status.Should().Be(TurnGameStatus.Ready);
        firstJoin.Value.Rounds.Should().BeEmpty();

        TurnGameHubResponse<TurnGameResponse> secondJoin =
            await partnerTwoConnection.InvokeAsync<TurnGameHubResponse<TurnGameResponse>>(
                "JoinTurnGame",
                scheduledGame.Id);
        TurnGameResponse firstPlayerView = await partnerOneStarted.Task.WaitAsync(
            TimeSpan.FromSeconds(5));

        secondJoin.IsSuccess.Should().BeTrue();
        secondJoin.Value!.Id.Should().Be(scheduledGame.Id);
        secondJoin.Value.Status.Should().Be(TurnGameStatus.InProgress);
        firstPlayerView.Id.Should().Be(secondJoin.Value.Id);
        firstPlayerView.Status.Should().Be(TurnGameStatus.InProgress);
        firstPlayerView.Rounds.Should().ContainSingle();
        secondJoin.Value.Rounds.Should().ContainSingle();
        firstPlayerView.Rounds.Single().Id.Should().Be(secondJoin.Value.Rounds.Single().Id);
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

    private HubConnection CreateHubConnection(string accessToken) =>
        new HubConnectionBuilder()
            .WithUrl(new Uri(_factory.Server.BaseAddress, "/hubs/turn-games"), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(accessToken);
                options.Transports = HttpTransportType.LongPolling;
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
            })
            .AddJsonProtocol(options =>
                options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
            .Build();
}

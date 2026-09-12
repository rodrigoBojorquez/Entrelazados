using DatePlanner.Application.Couples.BeginRelationship;
using DatePlanner.Application.Couples.GetCurrentCouple;
using DatePlanner.Application.Couples.EndRelationship;
using DatePlanner.Application.Couples.GetCurrentPartner;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.TurnGames;
using DatePlanner.Infrastructure.Database;
using DatePlanner.IntegrationTests.Abstractions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DatePlanner.IntegrationTests.Couples;

public sealed class CoupleTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task BeginRelationship_ShouldCreateCoupleVisibleToBothPartners()
    {
        RegisteredUser partnerOne = await RegisterUserAsync();
        RegisteredUser partnerTwo = await RegisterUserAsync();

        Result relationshipResult = await Sender.Send(
            new BeginRelationshipCommand(partnerOne.Id, partnerTwo.PartnerCode));

        Result<CurrentCoupleResponse> firstPartnerResult = await Sender.Send(
            new GetCurrentCoupleQuery(partnerOne.Id));
        Result<CurrentCoupleResponse> secondPartnerResult = await Sender.Send(
            new GetCurrentCoupleQuery(partnerTwo.Id));

        relationshipResult.IsSuccess.Should().BeTrue();
        firstPartnerResult.IsSuccess.Should().BeTrue();
        secondPartnerResult.IsSuccess.Should().BeTrue();
        firstPartnerResult.Value.Should().Be(secondPartnerResult.Value);
        firstPartnerResult.Value.PartnerOneId.Should().Be(partnerOne.Id);
        firstPartnerResult.Value.PartnerTwoId.Should().Be(partnerTwo.Id);
        firstPartnerResult.Value.IsActive.Should().BeTrue();

        DatePlannerDbContext dbContext = Services.GetRequiredService<DatePlannerDbContext>();
        TurnGame scheduledGame = await dbContext.TurnGames.SingleAsync(game =>
            game.CoupleId == firstPartnerResult.Value.Id &&
            game.Purpose == TurnGamePurpose.PlanningDecision);
        scheduledGame.Status.Should().Be(TurnGameStatus.AwaitingSelection);
        scheduledGame.GameType.Should().BeNull();

        RecordingTurnGameNotifier notifier =
            Services.GetRequiredService<RecordingTurnGameNotifier>();
        notifier.Notifications.Should().Contain(notification =>
            notification.TurnGameId == scheduledGame.Id &&
            notification.PartnerOneId == partnerOne.Id &&
            notification.PartnerTwoId == partnerTwo.Id);
    }

    [Fact]
    public async Task BeginRelationship_ShouldFail_WhenUserAlreadyHasPartner()
    {
        RegisteredCouple couple = await CreateCoupleAsync();
        RegisteredUser anotherUser = await RegisterUserAsync();

        Result result = await Sender.Send(new BeginRelationshipCommand(
            couple.PartnerOne.Id,
            anotherUser.PartnerCode));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Couple.UserAlreadyHasPartner");
    }

    [Fact]
    public async Task GetCurrentPartner_ShouldReturnPartnerAndPreferences()
    {
        RegisteredCouple couple = await CreateCoupleAsync();

        Result<GetCurrentPartnerResponse> result = await Sender.Send(
            new GetCurrentPartnerQuery(couple.PartnerOne.Id));

        result.IsSuccess.Should().BeTrue();
        result.Value.PartnerId.Should().Be(couple.PartnerTwo.Id);
        result.Value.PartnerName.Should().Be(
            $"{couple.PartnerTwo.FirstName} {couple.PartnerTwo.LastName}");
        result.Value.PartnerPreferences.FavoriteActivities.Should().BeEmpty();
        result.Value.PartnerPreferences.AvoidedActivities.Should().BeEmpty();
        result.Value.PartnerPreferences.FoodPreferences.Should().BeEmpty();
    }

    [Fact]
    public async Task EndRelationship_ShouldMakeBothPartnersAvailableToRelink()
    {
        RegisteredCouple couple = await CreateCoupleAsync();

        Result endResult = await Sender.Send(
            new EndRelationshipCommand(couple.PartnerOne.Id));
        Result<CurrentCoupleResponse> formerPartnerResult = await Sender.Send(
            new GetCurrentCoupleQuery(couple.PartnerTwo.Id));
        RegisteredUser anotherUser = await RegisterUserAsync();
        Result relinkResult = await Sender.Send(new BeginRelationshipCommand(
            couple.PartnerOne.Id,
            anotherUser.PartnerCode));

        endResult.IsSuccess.Should().BeTrue();
        formerPartnerResult.IsFailure.Should().BeTrue();
        formerPartnerResult.Error.Code.Should().Be("Couple.NotFound");
        relinkResult.IsSuccess.Should().BeTrue();
    }
}

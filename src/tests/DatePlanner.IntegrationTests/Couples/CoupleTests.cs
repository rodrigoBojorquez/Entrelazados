using DatePlanner.Application.Couples.BeginRelationship;
using DatePlanner.Application.Couples.GetCurrentCouple;
using DatePlanner.Domain.Abstractions;
using DatePlanner.IntegrationTests.Abstractions;
using FluentAssertions;

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
}

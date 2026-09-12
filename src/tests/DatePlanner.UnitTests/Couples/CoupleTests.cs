using DatePlanner.UnitTests.Abstractions;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Couples.Events;
using FluentAssertions;

namespace DatePlanner.UnitTests.Couples;

public class CoupleTests : BaseTest
{
    [Fact]
    public void Create_ShouldCreateActiveCouple_WhenPartnersAreDifferent()
    {
        Guid partnerOneId = Guid.NewGuid();
        Guid partnerTwoId = Guid.NewGuid();

        Result<Couple> result = Couple.Create(partnerOneId, partnerTwoId);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.PartnerOneId.Should().Be(partnerOneId);
        result.Value.PartnerTwoId.Should().Be(partnerTwoId);
        result.Value.IsActive.Should().BeTrue();
        result.Value.BeginRelationshipAt.Should().NotBe(default);
        RelationshipBeganDomainEvent domainEvent =
            AssertDomainEventWasPublished<RelationshipBeganDomainEvent>(result.Value);
        domainEvent.CoupleId.Should().Be(result.Value.Id);
        domainEvent.PartnerOneId.Should().Be(partnerOneId);
        domainEvent.PartnerTwoId.Should().Be(partnerTwoId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Create_ShouldRejectEmptyPartnerId_WithoutCreatingCouple(bool firstPartnerIsEmpty)
    {
        Guid validPartnerId = Guid.NewGuid();

        Result<Couple> result = firstPartnerIsEmpty
            ? Couple.Create(Guid.Empty, validPartnerId)
            : Couple.Create(validPartnerId, Guid.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.ShouldHaveCode("Couple.PartnerIdIsRequired", ErrorType.Problem);
    }

    [Fact]
    public void Create_ShouldRejectRelationshipWithSelf_WhenPartnerIdsMatch()
    {
        Guid partnerId = Guid.NewGuid();

        Result<Couple> result = Couple.Create(partnerId, partnerId);

        result.IsFailure.Should().BeTrue();
        result.Error.ShouldHaveCode("Couple.SamePersonCannotBePartners", ErrorType.Problem);
    }

    [Fact]
    public void EndRelationship_ShouldDeactivateActiveCouple()
    {
        Couple couple = Couple.Create(Guid.NewGuid(), Guid.NewGuid()).Value;

        Result result = couple.EndRelationship();

        result.IsSuccess.Should().BeTrue();
        couple.IsActive.Should().BeFalse();
    }

    [Fact]
    public void EndRelationship_ShouldFail_WhenCoupleIsAlreadyInactive()
    {
        Couple couple = Couple.Create(Guid.NewGuid(), Guid.NewGuid()).Value;
        couple.EndRelationship();

        Result result = couple.EndRelationship();

        result.IsFailure.Should().BeTrue();
        result.Error.ShouldHaveCode("Couple.RelationshipAlreadyEnded", ErrorType.Conflict);
    }
}

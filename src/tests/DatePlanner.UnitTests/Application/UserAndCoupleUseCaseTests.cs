using DatePlanner.Application.Couples.BeginRelationship;
using DatePlanner.Application.Users.RegisterUser;
using DatePlanner.Domain.Abstractions;
using DatePlanner.Domain.Couples;
using DatePlanner.Domain.Users;
using FluentAssertions;

namespace DatePlanner.UnitTests.Application;

public sealed class UserAndCoupleUseCaseTests
{
    [Fact]
    public async Task RegisterUser_ShouldNormalizeAndPersistNewUser()
    {
        var users = new FakeUserRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new RegisterUserCommandHandler(users, new FakePasswordHasher(), unitOfWork);

        Result<Guid> result = await handler.Handle(
            new RegisterUserCommand("  ANA@Example.com ", " Ana ", " Lopez ", "password"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        User user = users.Users.Should().ContainSingle().Subject;
        user.Id.Should().Be(result.Value);
        user.Email.Should().Be("ana@example.com");
        user.FullName.Should().Be("Ana Lopez");
        user.DatingPreferences.Should().NotBeNull();
        unitOfWork.SaveCalls.Should().Be(1);
    }

    [Fact]
    public async Task RegisterUser_ShouldNotPersist_WhenEmailAlreadyExists()
    {
        User existing = User.Create("ana@example.com", "Ana", "Lopez", "hash");
        var users = new FakeUserRepository(existing);
        var unitOfWork = new FakeUnitOfWork();
        var handler = new RegisterUserCommandHandler(users, new FakePasswordHasher(), unitOfWork);

        Result<Guid> result = await handler.Handle(
            new RegisterUserCommand("ANA@EXAMPLE.COM", "Other", "Name", "password"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.EmailAlreadyExists");
        users.Users.Should().ContainSingle();
        unitOfWork.SaveCalls.Should().Be(0);
    }

    [Fact]
    public async Task BeginRelationship_ShouldCreateCouple_WhenBothUsersAreAvailable()
    {
        User user = User.Create("one@example.com", "One", "User", "hash");
        User partner = User.Create("two@example.com", "Two", "User", "hash");
        var couples = new FakeCoupleRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new BeginRelationshipCommandHandler(
            new FakeUserRepository(user, partner),
            couples,
            unitOfWork);

        Result result = await handler.Handle(
            new BeginRelationshipCommand(user.Id, partner.PartnerCode),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        couples.Couples.Should().ContainSingle().Which.Should().Match<Couple>(couple =>
            couple.PartnerOneId == user.Id && couple.PartnerTwoId == partner.Id);
        unitOfWork.SaveCalls.Should().Be(1);
    }

    [Fact]
    public async Task BeginRelationship_ShouldRejectUserAlreadyInRelationship()
    {
        User user = User.Create("one@example.com", "One", "User", "hash");
        User partner = User.Create("two@example.com", "Two", "User", "hash");
        User currentPartner = User.Create("three@example.com", "Three", "User", "hash");
        Couple currentCouple = Couple.Create(user.Id, currentPartner.Id).Value;
        var couples = new FakeCoupleRepository(currentCouple);
        var unitOfWork = new FakeUnitOfWork();
        var handler = new BeginRelationshipCommandHandler(
            new FakeUserRepository(user, partner),
            couples,
            unitOfWork);

        Result result = await handler.Handle(
            new BeginRelationshipCommand(user.Id, partner.PartnerCode),
            CancellationToken.None);

        result.Error.Code.Should().Be("Couple.UserAlreadyHasPartner");
        couples.Couples.Should().ContainSingle();
        unitOfWork.SaveCalls.Should().Be(0);
    }
}

using DatePlanner.Application.Users.ChangeName;
using DatePlanner.Application.Users.GetUserProfile;
using DatePlanner.Application.Users.RegisterUser;
using DatePlanner.Domain.Abstractions;
using DatePlanner.IntegrationTests.Abstractions;
using FluentAssertions;

namespace DatePlanner.IntegrationTests.Users;

public sealed class UserTests(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task RegisterUser_ShouldPersistNormalizedUserAndProfile()
    {
        string email = $"  {Guid.NewGuid():N}@Example.COM  ";
        var command = new RegisterUserCommand(
            email,
            $"  {Faker.Name.FirstName()}  ",
            $"  {Faker.Name.LastName()}  ",
            "StrongPassword123!");

        Result<Guid> registrationResult = await Sender.Send(command);

        registrationResult.IsSuccess.Should().BeTrue();

        Result<UserProfileResponse> profileResult = await Sender.Send(
            new GetUserProfileQuery(registrationResult.Value));

        profileResult.IsSuccess.Should().BeTrue();
        profileResult.Value.Email.Should().Be(email.Trim().ToLowerInvariant());
        profileResult.Value.FirstName.Should().Be(command.FirstName.Trim());
        profileResult.Value.LastName.Should().Be(command.LastName.Trim());
        profileResult.Value.FullName.Should().Be(
            $"{command.FirstName.Trim()} {command.LastName.Trim()}");
        profileResult.Value.PartnerCode.Should().HaveLength(12);
        profileResult.Value.DatingPreferences.FavoriteActivities.Should().BeEmpty();
    }

    [Fact]
    public async Task RegisterUser_ShouldFail_WhenNormalizedEmailAlreadyExists()
    {
        RegisteredUser user = await RegisterUserAsync();

        Result<Guid> result = await Sender.Send(new RegisterUserCommand(
            $"  {user.Email.ToUpperInvariant()}  ",
            Faker.Name.FirstName(),
            Faker.Name.LastName(),
            user.Password));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("User.EmailAlreadyExists");
    }

    [Fact]
    public async Task ChangeName_ShouldBeVisibleInUserProfile()
    {
        RegisteredUser user = await RegisterUserAsync();
        string firstName = Faker.Name.FirstName();
        string lastName = Faker.Name.LastName();

        Result changeResult = await Sender.Send(
            new ChangeNameCommand(user.Id, firstName, lastName));
        Result<UserProfileResponse> profileResult = await Sender.Send(
            new GetUserProfileQuery(user.Id));

        changeResult.IsSuccess.Should().BeTrue();
        profileResult.IsSuccess.Should().BeTrue();
        profileResult.Value.FirstName.Should().Be(firstName);
        profileResult.Value.LastName.Should().Be(lastName);
        profileResult.Value.FullName.Should().Be($"{firstName} {lastName}");
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnValidationFailure_WhenInputIsInvalid()
    {
        Result<Guid> result = await Sender.Send(
            new RegisterUserCommand("invalid-email", "", "", "short"));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().NotBeEmpty();
    }
}

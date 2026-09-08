using Bogus;
using DatePlanner.Application.Couples.BeginRelationship;
using DatePlanner.Application.Couples.GetCurrentCouple;
using DatePlanner.Application.Users.GetUserProfile;
using DatePlanner.Application.Users.RegisterUser;
using DatePlanner.Domain.Abstractions;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DatePlanner.IntegrationTests.Abstractions;

[Collection(nameof(IntegrationTestCollection))]
public abstract class BaseIntegrationTest : IDisposable
{
    private readonly IServiceScope _scope;

    protected readonly ISender Sender;

    protected readonly Faker Faker = new();

    protected readonly IServiceProvider Services;

    public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Services = _scope.ServiceProvider;
        Sender = Services.GetRequiredService<ISender>();
    }

    protected async Task<RegisteredUser> RegisterUserAsync()
    {
        string email = $"integration-{Guid.NewGuid():N}@example.com";
        string password = "StrongPassword123!";
        string firstName = Faker.Name.FirstName();
        string lastName = Faker.Name.LastName();

        Result<Guid> registrationResult = await Sender.Send(
            new RegisterUserCommand(email, firstName, lastName, password));

        registrationResult.IsSuccess.Should().BeTrue();

        Result<UserProfileResponse> profileResult = await Sender.Send(
            new GetUserProfileQuery(registrationResult.Value));

        profileResult.IsSuccess.Should().BeTrue();

        return new RegisteredUser(
            registrationResult.Value,
            email,
            password,
            firstName,
            lastName,
            profileResult.Value.PartnerCode);
    }

    protected async Task<RegisteredCouple> CreateCoupleAsync()
    {
        RegisteredUser partnerOne = await RegisterUserAsync();
        RegisteredUser partnerTwo = await RegisterUserAsync();

        Result relationshipResult = await Sender.Send(
            new BeginRelationshipCommand(partnerOne.Id, partnerTwo.PartnerCode));

        relationshipResult.IsSuccess.Should().BeTrue();

        Result<CurrentCoupleResponse> coupleResult = await Sender.Send(
            new GetCurrentCoupleQuery(partnerOne.Id));

        coupleResult.IsSuccess.Should().BeTrue();

        return new RegisteredCouple(coupleResult.Value.Id, partnerOne, partnerTwo);
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    protected sealed record RegisteredUser(
        Guid Id,
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string PartnerCode);

    protected sealed record RegisteredCouple(
        Guid Id,
        RegisteredUser PartnerOne,
        RegisteredUser PartnerTwo);
}

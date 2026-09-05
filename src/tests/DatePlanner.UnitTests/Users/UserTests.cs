using DatePlanner.UnitTests.Abstractions;
using DatePlanner.Domain.Users;
using FluentAssertions;

namespace DatePlanner.UnitTests.Users;

public class UserTests : BaseTest
{
    [Fact]
    public void Create_ShouldPreserveIdentityData_AndGeneratePartnerCode()
    {
        const string email = "ana@example.com";
        const string firstName = "Ana";
        const string lastName = "Lopez";

        User user = User.Create(email, firstName, lastName, "password-hash");

        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be(email);
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.FullName.Should().Be("Ana Lopez");
        user.PartnerCode.Should().MatchRegex("^[A-Za-z0-9_-]{12}$");
    }

    [Fact]
    public void Create_ShouldGenerateDifferentPartnerCodes_ForDifferentUsers()
    {
        User first = User.Create("first@example.com", "First", "User", "hash");
        User second = User.Create("second@example.com", "Second", "User", "hash");

        second.PartnerCode.Should().NotBe(first.PartnerCode);
    }

    [Fact]
    public void ChangeName_ShouldReplaceBothNames_AndUpdateFullName()
    {
        User user = User.Create("ana@example.com", "Ana", "Lopez", "hash");

        user.ChangeName("Maria", "Santos");

        user.FirstName.Should().Be("Maria");
        user.LastName.Should().Be("Santos");
        user.FullName.Should().Be("Maria Santos");
    }
}

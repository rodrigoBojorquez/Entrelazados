using System.Security.Cryptography;
using System.Xml.XPath;
using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Users;

public class User : Entity
{
    private User()
    {
    }

    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    private string PasswordHash { get; set; }

    public string PartnerCode { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public DatingPreferences? DatingPreferences { get; private set; }

    public static User Create(string email, string firstName, string lastName, string passwordHash)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash,
            PartnerCode = GeneratePartnerCode()
        };

        return user;
    }

    public void ChangeName(string newFirstName, string newLastName)
    {
        FirstName = newFirstName;
        LastName = newLastName;
    }

    private static string GeneratePartnerCode()
    {
        Span<byte> bytes = stackalloc byte[9];
        RandomNumberGenerator.Fill(bytes);

        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
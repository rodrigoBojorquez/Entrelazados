using System.Security.Cryptography;
using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Users;

public class User : Entity
{
    private User()
    {
    }

    public Guid Id { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    private string PasswordHash { get; set; } = string.Empty;

    public string PartnerCode { get; private set; } = string.Empty;

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

        user.DatingPreferences = DatingPreferences.Create(user.Id);

        return user;
    }

    public void ChangeName(string newFirstName, string newLastName)
    {
        FirstName = newFirstName;
        LastName = newLastName;
    }

    public bool VerifyPassword(
        string password,
        Func<string, string, bool> passwordVerifier)
    {
        ArgumentNullException.ThrowIfNull(passwordVerifier);

        return passwordVerifier(password, PasswordHash);
    }

    public DatingPreferences GetOrCreateDatingPreferences()
    {
        DatingPreferences ??= DatingPreferences.Create(Id);

        return DatingPreferences;
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

using System.Security.Cryptography;
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

public class DatingPreferences : Entity
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    private readonly List<string> _favoriteActivities = [];

    public IReadOnlyCollection<string> FavoriteActivities => _favoriteActivities;

    private readonly List<string> _avoidedActivities = [];

    public IReadOnlyCollection<string> AvoidedActivities => _avoidedActivities;

    private readonly List<string> _foodPreferences = [];

    public IReadOnlyCollection<string> FoodPreferences => _foodPreferences;

    private DatingPreferences()
    {
    }

    public static DatingPreferences Create(Guid userId)
    {
        return new DatingPreferences
        {
            Id = Guid.NewGuid(),
            UserId = userId
        };
    }
    
    public void AddFavoriteActivity(string activity)
    {
        if (!_favoriteActivities.Contains(activity))
        {
            _favoriteActivities.Add(activity);
        }
    }
}
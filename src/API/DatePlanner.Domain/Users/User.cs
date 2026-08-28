using DatePlanner.Domain.Abstractions;

namespace DatePlanner.Domain.Users;

public class User : Entity
{
    private User() { }

    public static User Create(string email, string firstName, string lastName, string passwordHash)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash
        };

        return user;
    }
    
    public Guid Id { get; private set; }
    
    public string Email { get; private set; }
    
    public string FirstName { get; private set; }
    
    public string LastName { get; private set; }
    
    private string PasswordHash { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
}